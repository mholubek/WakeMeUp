from __future__ import annotations

from dataclasses import dataclass
import asyncio
import json
import logging
import os
from typing import Any

from aiohttp import ClientConnectionError, ClientError, ClientResponse, ClientSession
from homeassistant.core import HomeAssistant
from homeassistant.helpers.aiohttp_client import async_get_clientsession

from .const import (
    ADDON_API_PREFIX,
    ADDON_PORT,
    ADDON_SLUG,
    REQUEST_TIMEOUT_SECONDS,
    SUPERVISOR_ADDON_INFO_ENDPOINT,
)

_LOGGER = logging.getLogger(__name__)


@dataclass(slots=True)
class WakeMeUpAddonInfo:
    slug: str
    repository: str | None
    hostname: str | None
    ingress_port: int | None

    def resolved_hostname(self) -> str:
        if self.hostname:
            return self.hostname

        if self.repository:
            return f"{self.repository}_{self.slug}".replace("_", "-")

        raise WakeMeUpBridgeError(
            "WakeMeUp add-on hostname could not be resolved."
        )

    @property
    def api_base_url(self) -> str:
        return f"http://{self.resolved_hostname()}:{self.ingress_port or ADDON_PORT}{ADDON_API_PREFIX}"


class WakeMeUpBridgeError(Exception):
    """Raised when the bridge cannot reach the WakeMeUp add-on."""


@dataclass(slots=True)
class WakeMeUpProxyResponse:
    status: int
    body: bytes
    content_type: str | None


class WakeMeUpAddonClient:
    def __init__(self, hass: HomeAssistant) -> None:
        self._hass = hass
        self._session: ClientSession = async_get_clientsession(hass)
        self._supervisor_token = os.environ.get("SUPERVISOR_TOKEN")
        self._addon_info: WakeMeUpAddonInfo | None = None
        self._lock = asyncio.Lock()

    async def proxy_json_request(
        self,
        method: str,
        path: str,
        *,
        json_body: Any | None = None,
    ) -> WakeMeUpProxyResponse:
        if not self._supervisor_token:
            raise WakeMeUpBridgeError(
                "SUPERVISOR_TOKEN is not available. This bridge requires Home Assistant Supervisor."
            )

        addon_info = await self._async_get_addon_info()

        try:
            return await self._async_request(addon_info, method, path, json_body=json_body)
        except ClientConnectionError:
            addon_info = await self._async_get_addon_info(force_refresh=True)
            return await self._async_request(addon_info, method, path, json_body=json_body)
        except asyncio.TimeoutError as err:
            raise WakeMeUpBridgeError("WakeMeUp add-on request timed out.") from err
        except ClientError as err:
            raise WakeMeUpBridgeError("WakeMeUp add-on request failed.") from err

    async def async_get_alarms(self) -> list[dict[str, Any]]:
        response = await self.proxy_json_request("GET", "/alarms")
        payload = self._decode_json_response(response)
        if not isinstance(payload, list):
            raise WakeMeUpBridgeError("WakeMeUp add-on returned an invalid alarms response.")

        alarms: list[dict[str, Any]] = []
        for item in payload:
            if isinstance(item, dict):
                alarms.append(item)

        return alarms

    async def async_set_alarm_enabled(self, alarm_id: str, is_enabled: bool) -> None:
        response = await self.proxy_json_request(
            "PATCH",
            f"/alarms/{alarm_id}/enabled",
            json_body={"isEnabled": is_enabled},
        )
        self._ensure_success(response, f"updating alarm {alarm_id}")

    async def async_set_all_alarms_enabled(self, is_enabled: bool) -> int:
        alarms = await self.async_get_alarms()
        updated_count = 0

        for alarm in alarms:
            alarm_id = alarm.get("id")
            if not alarm_id:
                continue

            if bool(alarm.get("isEnabled")) == is_enabled:
                continue

            await self.async_set_alarm_enabled(str(alarm_id), is_enabled)
            updated_count += 1

        return updated_count

    async def _async_request(
        self,
        addon_info: WakeMeUpAddonInfo,
        method: str,
        path: str,
        *,
        json_body: Any | None = None,
    ) -> WakeMeUpProxyResponse:
        url = f"{addon_info.api_base_url}{path}"
        _LOGGER.debug("Proxying WakeMeUp request to %s %s", method, url)
        async with self._session.request(
            method,
            url,
            json=json_body,
            timeout=REQUEST_TIMEOUT_SECONDS,
        ) as response:
            return await self._build_proxy_response(response)

    async def _build_proxy_response(self, response: ClientResponse) -> WakeMeUpProxyResponse:
        body = await response.read()
        return WakeMeUpProxyResponse(
            status=response.status,
            body=body,
            content_type=response.content_type,
        )

    def _ensure_success(self, response: WakeMeUpProxyResponse, action: str) -> None:
        if response.status < 400:
            return

        details = self._extract_error_message(response)
        raise WakeMeUpBridgeError(
            f"WakeMeUp add-on returned HTTP {response.status} while {action}{details}."
        )

    def _decode_json_response(self, response: WakeMeUpProxyResponse) -> Any:
        self._ensure_success(response, "processing a JSON request")

        try:
            return json.loads(response.body.decode("utf-8"))
        except (UnicodeDecodeError, json.JSONDecodeError) as err:
            raise WakeMeUpBridgeError("WakeMeUp add-on returned invalid JSON.") from err

    def _extract_error_message(self, response: WakeMeUpProxyResponse) -> str:
        try:
            payload = json.loads(response.body.decode("utf-8"))
        except (UnicodeDecodeError, json.JSONDecodeError):
            return ""

        if isinstance(payload, dict):
            error = payload.get("error")
            if error:
                return f": {error}"

        return ""

    async def _async_get_addon_info(self, *, force_refresh: bool = False) -> WakeMeUpAddonInfo:
        async with self._lock:
            if self._addon_info is not None and not force_refresh:
                return self._addon_info

            headers = {"Authorization": f"Bearer {self._supervisor_token}"}
            async with self._session.get(SUPERVISOR_ADDON_INFO_ENDPOINT, headers=headers, timeout=REQUEST_TIMEOUT_SECONDS) as response:
                response.raise_for_status()
                payload = await response.json()

            addon = payload.get("data")
            if not addon:
                raise WakeMeUpBridgeError(
                    "WakeMeUp add-on details could not be loaded from Supervisor."
                )

            if not addon.get("installed"):
                raise WakeMeUpBridgeError("WakeMeUp add-on is not installed.")

            if addon.get("state") != "started":
                raise WakeMeUpBridgeError("WakeMeUp add-on is not started.")

            hostname = addon.get("hostname")
            repository = addon.get("repository")
            ingress_port = addon.get("ingress_port")

            if not hostname and not repository:
                raise WakeMeUpBridgeError(
                    "WakeMeUp add-on hostname could not be resolved from Supervisor."
                )

            self._addon_info = WakeMeUpAddonInfo(
                slug=ADDON_SLUG,
                repository=repository,
                hostname=hostname,
                ingress_port=ingress_port,
            )
            _LOGGER.debug(
                "Resolved WakeMeUp add-on upstream hostname=%s ingress_port=%s repository=%s",
                hostname,
                ingress_port,
                repository,
            )
            return self._addon_info

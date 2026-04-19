from __future__ import annotations

from dataclasses import dataclass
import asyncio
import json
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
    SUPERVISOR_ADDONS_ENDPOINT,
)


@dataclass(slots=True)
class WakeMeUpAddonInfo:
    slug: str
    repository: str

    @property
    def hostname(self) -> str:
        return f"{self.repository}_{self.slug}".replace("_", "-")

    @property
    def api_base_url(self) -> str:
        return f"http://{self.hostname}:{ADDON_PORT}{ADDON_API_PREFIX}"


class WakeMeUpBridgeError(Exception):
    """Raised when the bridge cannot reach the WakeMeUp add-on."""


@dataclass(slots=True)
class WakeMeUpProxyResponse:
    status: int
    body: bytes
    content_type: str | None

    def as_json(self) -> Any:
        return json.loads(self.body.decode("utf-8"))


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

    async def _async_request(
        self,
        addon_info: WakeMeUpAddonInfo,
        method: str,
        path: str,
        *,
        json_body: Any | None = None,
    ) -> WakeMeUpProxyResponse:
        url = f"{addon_info.api_base_url}{path}"
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

    async def _async_get_addon_info(self, *, force_refresh: bool = False) -> WakeMeUpAddonInfo:
        async with self._lock:
            if self._addon_info is not None and not force_refresh:
                return self._addon_info

            headers = {"Authorization": f"Bearer {self._supervisor_token}"}
            async with self._session.get(
                SUPERVISOR_ADDONS_ENDPOINT,
                headers=headers,
                timeout=REQUEST_TIMEOUT_SECONDS,
            ) as response:
                response.raise_for_status()
                payload = await response.json()

            addons = payload.get("data", {}).get("addons", [])
            addon = next(
                (
                    item
                    for item in addons
                    if item.get("slug") == ADDON_SLUG
                    and item.get("installed")
                    and item.get("state") == "started"
                ),
                None,
            )

            if addon is None:
                raise WakeMeUpBridgeError(
                    "WakeMeUp add-on is not installed or not started."
                )

            repository = addon.get("repository")
            if not repository:
                raise WakeMeUpBridgeError(
                    "WakeMeUp add-on repository identifier could not be resolved."
                )

            self._addon_info = WakeMeUpAddonInfo(
                slug=ADDON_SLUG,
                repository=repository,
            )
            return self._addon_info

from __future__ import annotations

import json
from typing import Any

from aiohttp.web import Request, Response
from homeassistant.components.http import HomeAssistantView
from homeassistant.core import HomeAssistant

from .addon_client import WakeMeUpAddonClient, WakeMeUpBridgeError, WakeMeUpProxyResponse
from .const import API_BASE_PATH, DATA_ADDON_CLIENT, DOMAIN


def async_register_views(hass: HomeAssistant) -> None:
    hass.http.register_view(WakeMeUpMetaView(hass))
    hass.http.register_view(WakeMeUpAlarmsView(hass))
    hass.http.register_view(WakeMeUpAlarmsEnabledView(hass))
    hass.http.register_view(WakeMeUpAlarmDetailView(hass))
    hass.http.register_view(WakeMeUpAlarmEnabledView(hass))
    hass.http.register_view(WakeMeUpDebugUpstreamView(hass))


class WakeMeUpBaseView(HomeAssistantView):
    requires_auth = True

    def __init__(self, hass: HomeAssistant) -> None:
        self._hass = hass

    @property
    def _client(self) -> WakeMeUpAddonClient:
        return self._hass.data[DOMAIN][DATA_ADDON_CLIENT]

    async def _proxy(
        self,
        method: str,
        path: str,
        request: Request,
    ) -> Response:
        try:
            payload = await request.json() if request.can_read_body else None
        except json.JSONDecodeError:
            return self.json_message(
                "Request body must contain valid JSON.",
                status_code=400,
            )

        try:
            proxy_response = await self._client.proxy_json_request(
                method,
                path,
                json_body=payload,
            )
        except WakeMeUpBridgeError as err:
            return self.json(
                {
                    "error": str(err),
                    "details": [err.details] if getattr(err, "details", None) else [],
                },
                status_code=502,
            )

        return _to_response(proxy_response)


class WakeMeUpMetaView(WakeMeUpBaseView):
    url = f"{API_BASE_PATH}/meta"
    name = "api:wake_me_up:meta"

    async def get(self, request: Request) -> Response:
        return await self._proxy("GET", "/meta", request)


class WakeMeUpAlarmsView(WakeMeUpBaseView):
    url = f"{API_BASE_PATH}/alarms"
    name = "api:wake_me_up:alarms"

    async def get(self, request: Request) -> Response:
        return await self._proxy("GET", "/alarms", request)

    async def post(self, request: Request) -> Response:
        return await self._proxy("POST", "/alarms", request)


class WakeMeUpAlarmsEnabledView(WakeMeUpBaseView):
    url = f"{API_BASE_PATH}/alarms/enabled"
    name = "api:wake_me_up:alarms_enabled"

    async def patch(self, request: Request) -> Response:
        try:
            payload = await request.json()
        except json.JSONDecodeError:
            return self.json_message(
                "Request body must contain valid JSON.",
                status_code=400,
            )

        if not isinstance(payload, dict) or "isEnabled" not in payload:
            return self.json(
                {
                    "error": "Request body must contain the 'isEnabled' field.",
                    "details": [],
                },
                status_code=400,
            )

        is_enabled = payload["isEnabled"]
        if not isinstance(is_enabled, bool):
            return self.json(
                {
                    "error": "The 'isEnabled' field must be a boolean.",
                    "details": [],
                },
                status_code=400,
            )

        try:
            updated_count = await self._client.async_set_all_alarms_enabled(is_enabled)
        except WakeMeUpBridgeError as err:
            return self.json(
                {
                    "error": str(err),
                    "details": [],
                },
                status_code=502,
            )

        return self.json(
            {
                "isEnabled": is_enabled,
                "updatedCount": updated_count,
            }
        )


class WakeMeUpAlarmDetailView(WakeMeUpBaseView):
    url = f"{API_BASE_PATH}/alarms/{{alarm_id}}"
    name = "api:wake_me_up:alarm_detail"

    async def get(self, request: Request, alarm_id: str) -> Response:
        return await self._proxy("GET", f"/alarms/{alarm_id}", request)

    async def put(self, request: Request, alarm_id: str) -> Response:
        return await self._proxy("PUT", f"/alarms/{alarm_id}", request)

    async def delete(self, request: Request, alarm_id: str) -> Response:
        return await self._proxy("DELETE", f"/alarms/{alarm_id}", request)


class WakeMeUpAlarmEnabledView(WakeMeUpBaseView):
    url = f"{API_BASE_PATH}/alarms/{{alarm_id}}/enabled"
    name = "api:wake_me_up:alarm_enabled"

    async def patch(self, request: Request, alarm_id: str) -> Response:
        return await self._proxy("PATCH", f"/alarms/{alarm_id}/enabled", request)


class WakeMeUpDebugUpstreamView(WakeMeUpBaseView):
    url = f"{API_BASE_PATH}/debug/upstream"
    name = "api:wake_me_up:debug_upstream"

    async def get(self, request: Request) -> Response:
        return self.json(await self._client.async_get_debug_info())


def _to_response(proxy_response: WakeMeUpProxyResponse) -> Response:
    return Response(
        body=proxy_response.body,
        status=proxy_response.status,
        content_type=proxy_response.content_type or "application/json",
    )

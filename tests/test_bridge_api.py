from __future__ import annotations

import json
import sys
import types
import unittest
from types import SimpleNamespace
from unittest.mock import AsyncMock


def _install_homeassistant_stubs() -> None:
    if "homeassistant" in sys.modules:
        return

    aiohttp = types.ModuleType("aiohttp")

    class ClientConnectionError(Exception):
        pass

    class ClientError(Exception):
        pass

    class ClientResponse:
        pass

    class ClientSession:
        pass

    aiohttp.ClientConnectionError = ClientConnectionError
    aiohttp.ClientError = ClientError
    aiohttp.ClientResponse = ClientResponse
    aiohttp.ClientSession = ClientSession
    sys.modules["aiohttp"] = aiohttp

    aiohttp_web = types.ModuleType("aiohttp.web")

    class Response:
        def __init__(self, *, body=None, status=200, content_type=None):
            self.body = body
            self.status = status
            self.content_type = content_type

    aiohttp_web.Request = object
    aiohttp_web.Response = Response
    sys.modules["aiohttp.web"] = aiohttp_web

    homeassistant = types.ModuleType("homeassistant")
    homeassistant.__path__ = []
    sys.modules["homeassistant"] = homeassistant

    components = types.ModuleType("homeassistant.components")
    components.__path__ = []
    sys.modules["homeassistant.components"] = components

    http = types.ModuleType("homeassistant.components.http")

    class HomeAssistantView:
        requires_auth = True

        def json(self, data, status_code=200):
            return SimpleNamespace(status=status_code, data=data)

        def json_message(self, message, status_code=200):
            return SimpleNamespace(status=status_code, data={"message": message})

    http.HomeAssistantView = HomeAssistantView
    sys.modules["homeassistant.components.http"] = http

    core = types.ModuleType("homeassistant.core")
    core.HomeAssistant = object
    sys.modules["homeassistant.core"] = core

    aiohttp_client = types.ModuleType("homeassistant.helpers.aiohttp_client")
    aiohttp_client.async_get_clientsession = lambda hass: None
    sys.modules["homeassistant.helpers.aiohttp_client"] = aiohttp_client


_install_homeassistant_stubs()

from custom_components.wakemeup_bridge.api import WakeMeUpAlarmsEnabledView
from custom_components.wakemeup_bridge.addon_client import WakeMeUpAddonClient, WakeMeUpBridgeError
from custom_components.wakemeup_bridge.const import DATA_ADDON_CLIENT, DOMAIN


class WakeMeUpBridgeApiTests(unittest.IsolatedAsyncioTestCase):
    async def test_bulk_endpoint_rejects_missing_field(self) -> None:
        client = SimpleNamespace(async_set_all_alarms_enabled=AsyncMock())
        hass = SimpleNamespace(data={DOMAIN: {DATA_ADDON_CLIENT: client}})
        view = WakeMeUpAlarmsEnabledView(hass)
        request = SimpleNamespace(json=AsyncMock(return_value={}))

        response = await view.patch(request)

        self.assertEqual(response.status, 400)
        self.assertEqual(
            response.data["error"],
            "Request body must contain the 'isEnabled' field.",
        )
        client.async_set_all_alarms_enabled.assert_not_awaited()

    async def test_bulk_endpoint_rejects_non_boolean_field(self) -> None:
        client = SimpleNamespace(async_set_all_alarms_enabled=AsyncMock())
        hass = SimpleNamespace(data={DOMAIN: {DATA_ADDON_CLIENT: client}})
        view = WakeMeUpAlarmsEnabledView(hass)
        request = SimpleNamespace(json=AsyncMock(return_value={"isEnabled": "yes"}))

        response = await view.patch(request)

        self.assertEqual(response.status, 400)
        self.assertEqual(
            response.data["error"],
            "The 'isEnabled' field must be a boolean.",
        )
        client.async_set_all_alarms_enabled.assert_not_awaited()

    async def test_bulk_endpoint_returns_updated_count(self) -> None:
        client = SimpleNamespace(async_set_all_alarms_enabled=AsyncMock(return_value=3))
        hass = SimpleNamespace(data={DOMAIN: {DATA_ADDON_CLIENT: client}})
        view = WakeMeUpAlarmsEnabledView(hass)
        request = SimpleNamespace(json=AsyncMock(return_value={"isEnabled": True}))

        response = await view.patch(request)

        self.assertEqual(response.status, 200)
        self.assertEqual(response.data, {"isEnabled": True, "updatedCount": 3})
        client.async_set_all_alarms_enabled.assert_awaited_once_with(True)

    async def test_to_response_preserves_json_body(self) -> None:
        from custom_components.wakemeup_bridge.api import _to_response
        from custom_components.wakemeup_bridge.addon_client import WakeMeUpProxyResponse

        response = _to_response(
            WakeMeUpProxyResponse(
                status=200,
                body=json.dumps({"ok": True}).encode("utf-8"),
                content_type="application/json",
            )
        )

        self.assertEqual(response.status, 200)
        self.assertEqual(response.content_type, "application/json")

    async def test_addon_client_builds_info_from_direct_supervisor_payload(self) -> None:
        client = WakeMeUpAddonClient(SimpleNamespace())

        addon_info = client._build_addon_info(
            {
                "slug": "wakemeup",
                "name": "WakeMeUp",
                "installed": True,
                "state": "started",
                "hostname": "local-wakemeup",
                "repository": "local",
                "ingress_port": 8080,
            }
        )

        self.assertEqual(addon_info.hostname, "local-wakemeup")
        self.assertEqual(addon_info.repository, "local")
        self.assertEqual(addon_info.ingress_port, 8080)

    async def test_addon_client_raises_controlled_error_for_missing_addon(self) -> None:
        client = WakeMeUpAddonClient(SimpleNamespace())

        with self.assertRaises(WakeMeUpBridgeError):
            client._build_addon_info(
                {
                    "slug": "wakemeup",
                    "name": "WakeMeUp",
                    "installed": True,
                    "state": "started",
                    "hostname": None,
                    "repository": None,
                    "ingress_port": None,
                }
            )

    async def test_addon_client_unwraps_supervisor_data_payload(self) -> None:
        client = WakeMeUpAddonClient(SimpleNamespace())

        unwrapped = client._unwrap_supervisor_payload(
            {
                "result": "ok",
                "data": {
                    "addons": [
                        {
                            "slug": "local_wakemeup",
                            "name": "WakeMeUp",
                        }
                    ]
                },
            }
        )

        self.assertIn("addons", unwrapped)
        self.assertEqual(unwrapped["addons"][0]["slug"], "local_wakemeup")


if __name__ == "__main__":
    unittest.main()

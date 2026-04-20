from __future__ import annotations

import sys
import types
import unittest
from dataclasses import dataclass
from types import SimpleNamespace


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

    const = types.ModuleType("homeassistant.const")

    class Platform:
        BINARY_SENSOR = "binary_sensor"
        BUTTON = "button"

    const.Platform = Platform
    sys.modules["homeassistant.const"] = const

    config_entries = types.ModuleType("homeassistant.config_entries")
    config_entries.ConfigEntry = object
    sys.modules["homeassistant.config_entries"] = config_entries

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

    binary_sensor = types.ModuleType("homeassistant.components.binary_sensor")

    class BinarySensorEntity:
        async def async_added_to_hass(self):
            return None

        def async_on_remove(self, func):
            self._remove = func

        def async_write_ha_state(self):
            self._state_written = True

    binary_sensor.BinarySensorEntity = BinarySensorEntity
    sys.modules["homeassistant.components.binary_sensor"] = binary_sensor

    button = types.ModuleType("homeassistant.components.button")

    class ButtonEntity:
        pass

    button.ButtonEntity = ButtonEntity
    sys.modules["homeassistant.components.button"] = button

    core = types.ModuleType("homeassistant.core")
    core.HomeAssistant = object
    core.Event = object
    core.callback = lambda func: func
    sys.modules["homeassistant.core"] = core

    dispatcher = types.ModuleType("homeassistant.helpers.dispatcher")
    dispatcher.async_dispatcher_connect = lambda hass, signal, callback: lambda: None
    dispatcher.async_dispatcher_send = lambda hass, signal: None
    sys.modules["homeassistant.helpers.dispatcher"] = dispatcher

    entity_platform = types.ModuleType("homeassistant.helpers.entity_platform")
    entity_platform.AddEntitiesCallback = object
    sys.modules["homeassistant.helpers.entity_platform"] = entity_platform

    entity = types.ModuleType("homeassistant.helpers.entity")

    @dataclass
    class DeviceInfo:
        identifiers: set[tuple[str, str]]
        name: str
        manufacturer: str
        model: str

    class Entity:
        _attr_has_entity_name = False

    class EntityCategory:
        CONFIG = "config"

    entity.DeviceInfo = DeviceInfo
    entity.Entity = Entity
    entity.EntityCategory = EntityCategory
    sys.modules["homeassistant.helpers.entity"] = entity

    event = types.ModuleType("homeassistant.helpers.event")
    event.async_call_later = lambda hass, delay, callback: lambda: None
    sys.modules["homeassistant.helpers.event"] = event

    util = types.ModuleType("homeassistant.util")
    sys.modules["homeassistant.util"] = util
    dt = types.ModuleType("homeassistant.util.dt")
    dt.utcnow = lambda: None
    sys.modules["homeassistant.util.dt"] = dt

    aiohttp_client = types.ModuleType("homeassistant.helpers.aiohttp_client")
    aiohttp_client.async_get_clientsession = lambda hass: None
    sys.modules["homeassistant.helpers.aiohttp_client"] = aiohttp_client


_install_homeassistant_stubs()

from custom_components.wakemeup_bridge.binary_sensor import WakeMeUpAlarmTriggeredBinarySensor
from custom_components.wakemeup_bridge.button import WakeMeUpBulkAlarmButton


class WakeMeUpBridgeEntityTests(unittest.TestCase):
    def test_alarm_triggered_sensor_has_fixed_english_name(self) -> None:
        entry = SimpleNamespace(entry_id="entry-1")
        runtime = SimpleNamespace(state=SimpleNamespace(is_triggered=False, last_alarm_name="", last_alarm_time="", last_alarm_description="", triggered_at=None))

        entity = WakeMeUpAlarmTriggeredBinarySensor(entry, runtime)

        self.assertEqual(entity._attr_name, "Alarm triggered")
        self.assertEqual(entity._attr_unique_id, "entry-1_alarm_triggered")

    def test_bulk_buttons_have_fixed_english_names(self) -> None:
        entry = SimpleNamespace(entry_id="entry-1")
        client = SimpleNamespace()

        enable_entity = WakeMeUpBulkAlarmButton(entry, client, True)
        disable_entity = WakeMeUpBulkAlarmButton(entry, client, False)

        self.assertEqual(enable_entity._attr_name, "Enable all alarms")
        self.assertEqual(disable_entity._attr_name, "Disable all alarms")
        self.assertEqual(enable_entity._attr_unique_id, "entry-1_enable_all_alarms")
        self.assertEqual(disable_entity._attr_unique_id, "entry-1_disable_all_alarms")


if __name__ == "__main__":
    unittest.main()

from __future__ import annotations

from homeassistant.components.binary_sensor import BinarySensorEntity
from homeassistant.config_entries import ConfigEntry
from homeassistant.core import HomeAssistant
from homeassistant.helpers.dispatcher import async_dispatcher_connect
from homeassistant.helpers.entity_platform import AddEntitiesCallback

from .const import DATA_RUNTIME, DOMAIN, SIGNAL_STATE_UPDATED
from .entity import WakeMeUpBridgeEntity
from .runtime import WakeMeUpRuntime


async def async_setup_entry(
    hass: HomeAssistant,
    entry: ConfigEntry,
    async_add_entities: AddEntitiesCallback,
) -> None:
    runtime: WakeMeUpRuntime = hass.data[DOMAIN][DATA_RUNTIME]
    async_add_entities([WakeMeUpAlarmTriggeredBinarySensor(entry, runtime)])


class WakeMeUpAlarmTriggeredBinarySensor(WakeMeUpBridgeEntity, BinarySensorEntity):
    _attr_name = "Alarm triggered"
    _attr_icon = "mdi:alarm-light"
    _attr_entity_registry_enabled_default = True

    def __init__(self, entry: ConfigEntry, runtime: WakeMeUpRuntime) -> None:
        super().__init__(entry)
        self._runtime = runtime
        self._attr_unique_id = f"{entry.entry_id}_alarm_triggered"

    @property
    def is_on(self) -> bool:
        return self._runtime.state.is_triggered

    @property
    def extra_state_attributes(self) -> dict[str, str]:
        state = self._runtime.state
        attributes: dict[str, str] = {
            "last_alarm_name": state.last_alarm_name,
            "last_alarm_time": state.last_alarm_time,
            "last_alarm_description": state.last_alarm_description,
        }

        if state.triggered_at is not None:
            attributes["last_triggered_at"] = state.triggered_at.isoformat()

        return attributes

    async def async_added_to_hass(self) -> None:
        await super().async_added_to_hass()
        self.async_on_remove(
            async_dispatcher_connect(
                self.hass,
                SIGNAL_STATE_UPDATED,
                self._handle_runtime_update,
            )
        )

    def _handle_runtime_update(self) -> None:
        self.async_write_ha_state()

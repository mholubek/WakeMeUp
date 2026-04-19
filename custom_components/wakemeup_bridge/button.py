from __future__ import annotations

from homeassistant.components.button import ButtonEntity
from homeassistant.config_entries import ConfigEntry
from homeassistant.core import HomeAssistant
from homeassistant.helpers.entity import EntityCategory
from homeassistant.helpers.entity_platform import AddEntitiesCallback

from .addon_client import WakeMeUpAddonClient
from .const import DATA_ADDON_CLIENT, DOMAIN
from .entity import WakeMeUpBridgeEntity


async def async_setup_entry(
    hass: HomeAssistant,
    entry: ConfigEntry,
    async_add_entities: AddEntitiesCallback,
) -> None:
    client: WakeMeUpAddonClient = hass.data[DOMAIN][DATA_ADDON_CLIENT]
    async_add_entities(
        [
            WakeMeUpBulkAlarmButton(entry, client, True),
            WakeMeUpBulkAlarmButton(entry, client, False),
        ]
    )


class WakeMeUpBulkAlarmButton(WakeMeUpBridgeEntity, ButtonEntity):
    _attr_entity_category = EntityCategory.CONFIG

    def __init__(
        self,
        entry: ConfigEntry,
        client: WakeMeUpAddonClient,
        enable_alarms: bool,
    ) -> None:
        super().__init__(entry)
        self._client = client
        self._enable_alarms = enable_alarms

        action = "enable" if enable_alarms else "disable"
        self._attr_unique_id = f"{entry.entry_id}_{action}_all_alarms"
        self._attr_name = "Enable all alarms" if enable_alarms else "Disable all alarms"
        self._attr_icon = "mdi:alarm-check" if enable_alarms else "mdi:alarm-off"

    async def async_press(self) -> None:
        await self._client.async_set_all_alarms_enabled(self._enable_alarms)

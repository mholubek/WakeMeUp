from __future__ import annotations

from homeassistant.config_entries import ConfigEntry
from homeassistant.core import HomeAssistant

from .addon_client import WakeMeUpAddonClient
from .api import async_register_views
from .const import DATA_ADDON_CLIENT, DATA_VIEWS_REGISTERED, DOMAIN


async def async_setup(hass: HomeAssistant, config: dict) -> bool:
    hass.data.setdefault(DOMAIN, {})
    await _async_setup_bridge(hass)
    return True


async def async_setup_entry(hass: HomeAssistant, entry: ConfigEntry) -> bool:
    hass.data.setdefault(DOMAIN, {})
    await _async_setup_bridge(hass)
    return True


async def async_unload_entry(hass: HomeAssistant, entry: ConfigEntry) -> bool:
    return True


async def _async_setup_bridge(hass: HomeAssistant) -> None:
    domain_data = hass.data.setdefault(DOMAIN, {})

    if DATA_ADDON_CLIENT not in domain_data:
        domain_data[DATA_ADDON_CLIENT] = WakeMeUpAddonClient(hass)

    if not domain_data.get(DATA_VIEWS_REGISTERED):
        async_register_views(hass)
        domain_data[DATA_VIEWS_REGISTERED] = True

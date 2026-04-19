from __future__ import annotations

from homeassistant.config_entries import ConfigEntry
from homeassistant.core import HomeAssistant

from .addon_client import WakeMeUpAddonClient
from .api import async_register_views
from .const import DATA_ADDON_CLIENT, DATA_RUNTIME, DATA_VIEWS_REGISTERED, DOMAIN, PLATFORMS
from .runtime import WakeMeUpRuntime


async def async_setup(hass: HomeAssistant, config: dict) -> bool:
    hass.data.setdefault(DOMAIN, {})
    await _async_setup_bridge(hass)
    return True


async def async_setup_entry(hass: HomeAssistant, entry: ConfigEntry) -> bool:
    hass.data.setdefault(DOMAIN, {})
    await _async_setup_bridge(hass)
    domain_data = hass.data[DOMAIN]

    runtime = domain_data.get(DATA_RUNTIME)
    if runtime is None:
        runtime = WakeMeUpRuntime(hass)
        await runtime.async_setup()
        domain_data[DATA_RUNTIME] = runtime

    await hass.config_entries.async_forward_entry_setups(entry, PLATFORMS)
    return True


async def async_unload_entry(hass: HomeAssistant, entry: ConfigEntry) -> bool:
    unload_ok = await hass.config_entries.async_unload_platforms(entry, PLATFORMS)
    if not unload_ok:
        return False

    domain_data = hass.data.get(DOMAIN, {})
    runtime: WakeMeUpRuntime | None = domain_data.pop(DATA_RUNTIME, None)
    if runtime is not None:
        await runtime.async_unload()

    return True


async def _async_setup_bridge(hass: HomeAssistant) -> None:
    domain_data = hass.data.setdefault(DOMAIN, {})

    if DATA_ADDON_CLIENT not in domain_data:
        domain_data[DATA_ADDON_CLIENT] = WakeMeUpAddonClient(hass)

    if not domain_data.get(DATA_VIEWS_REGISTERED):
        async_register_views(hass)
        domain_data[DATA_VIEWS_REGISTERED] = True

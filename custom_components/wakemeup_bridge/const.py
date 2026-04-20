from __future__ import annotations

from homeassistant.const import Platform

DOMAIN = "wakemeup_bridge"
API_BASE_PATH = "/api/wakemeup"
EVENT_ALARM_TRIGGERED = "wakemeup_alarm_triggered"

ADDON_NAME = "WakeMeUp"
ADDON_PORT = 8080
ADDON_API_PREFIX = "/api/v1"

SUPERVISOR_URL = "http://supervisor"
SUPERVISOR_ADDONS_ENDPOINT = f"{SUPERVISOR_URL}/addons"
SUPERVISOR_ADDON_INFO_ENDPOINT_TEMPLATE = f"{SUPERVISOR_URL}/addons/{{addon_slug}}/info"

REQUEST_TIMEOUT_SECONDS = 10
DATA_ADDON_CLIENT = "addon_client"
DATA_VIEWS_REGISTERED = "views_registered"
DATA_RUNTIME = "runtime"
TRIGGER_RESET_SECONDS = 30
SIGNAL_STATE_UPDATED = f"{DOMAIN}_state_updated"
PLATFORMS: list[Platform] = [Platform.BINARY_SENSOR, Platform.BUTTON]

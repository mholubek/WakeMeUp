# WakeMeUp Bridge

`wakemeup_bridge` is the Home Assistant custom integration that connects the WakeMeUp add-on with Home Assistant dashboards, automations, and future HACS/Lovelace UI components.

It gives WakeMeUp a stable Home Assistant-facing API and a small set of helper entities, so frontend code does not need to know the add-on ingress URL or implement its own add-on discovery.

[![Open your Home Assistant instance and start the WakeMeUp Bridge setup flow](https://my.home-assistant.io/badges/config_flow_start.svg)](https://my.home-assistant.io/redirect/config_flow_start?domain=wakemeup_bridge)

## What It Does

The bridge exposes authenticated Home Assistant API routes under `/api/wakemeup/...`:

- `GET /api/wakemeup/meta`
- `GET /api/wakemeup/alarms`
- `POST /api/wakemeup/alarms`
- `PATCH /api/wakemeup/alarms/enabled`
- `GET /api/wakemeup/alarms/{id}`
- `PUT /api/wakemeup/alarms/{id}`
- `DELETE /api/wakemeup/alarms/{id}`
- `PATCH /api/wakemeup/alarms/{id}/enabled`

It also creates these Home Assistant entities:

- `binary_sensor.wakemeup_bridge_alarm_triggered`
- `button.enable_all_alarms`
- `button.disable_all_alarms`

Entity names stay fixed in English by design. Any UI localization for a HACS/Lovelace card should be handled in the frontend component, not in the bridge.

## How It Works

The bridge:

1. Uses the Home Assistant Supervisor API to detect the installed WakeMeUp add-on automatically.
2. Resolves the add-on slug and internal hostname without manual configuration.
3. Proxies requests from Home Assistant `/api/wakemeup/...` routes to the add-on `/api/v1/...` API.
4. Listens for the `wakemeup_alarm_triggered` event and mirrors it into a binary sensor that stays `on` for 30 seconds.

This means a Lovelace or HACS component can talk only to Home Assistant and never needs to know the add-on ingress path.

## Installation

### Manual

1. Copy [custom_components/wakemeup_bridge](./) to your Home Assistant config directory as:
   - `config/custom_components/wakemeup_bridge`
2. Restart Home Assistant.
3. Open `Settings > Devices & Services`.
4. Click `Add Integration`.
5. Search for `WakeMeUp Bridge`.
6. Finish the setup flow.
7. Make sure the WakeMeUp add-on is installed and started.

### Quick Start in Home Assistant

After the files are installed, you can start the config flow directly with the button at the top of this README.

## Helper Entities

### `binary_sensor.wakemeup_bridge_alarm_triggered`

- Turns `on` for 30 seconds after WakeMeUp fires an alarm
- Exposes attributes:
  - `last_alarm_name`
  - `last_alarm_time`
  - `last_alarm_description`
  - `last_triggered_at`

### `button.enable_all_alarms`

- Enables all WakeMeUp alarms through the bridge

### `button.disable_all_alarms`

- Disables all WakeMeUp alarms through the bridge

## Intended Frontend Contract

Frontend code should use:

```ts
const API_BASE = "/api/wakemeup";
```

The bridge keeps payloads compatible with the WakeMeUp add-on API, so existing CRUD request and response handling can stay the same.

Bulk enable and disable uses:

```json
{
  "isEnabled": true
}
```

and returns:

```json
{
  "isEnabled": true,
  "updatedCount": 3
}
```

## Notes

- The WakeMeUp add-on must be running.
- The bridge is optional for the add-on itself, but recommended for dashboard/frontend integration.
- The bridge was smoke-tested against live Home Assistant routes for `GET`, `POST`, `PUT`, `PATCH`, and `DELETE` alarm operations.

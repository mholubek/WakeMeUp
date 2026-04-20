# WakeMeUp Add-on Repository

This repository contains the Home Assistant add-on for WakeMeUp.

WakeMeUp is a Home Assistant alarm application built with ASP.NET 10 Blazor Server. It lets users create one-time or repeating alarms, stores data in SQLite, and emits the `wakemeup_alarm_triggered` event when an alarm becomes due.

This project is fully vibe coded.

[![Open your Home Assistant instance and add this repository](https://my.home-assistant.io/badges/supervisor_add_addon_repository.svg)](https://my.home-assistant.io/redirect/supervisor_add_addon_repository/?repository_url=https%3A%2F%2Fgithub.com%2Fmholubek%2FWakeMeUp)
[![Build WakeMeUp Add-on](https://github.com/mholubek/WakeMeUp/actions/workflows/build-addon.yml/badge.svg)](https://github.com/mholubek/WakeMeUp/actions/workflows/build-addon.yml)

## Repository Layout

- [repository.yaml](./repository.yaml) - Home Assistant add-on repository metadata
- [wakemeup](./wakemeup) - the WakeMeUp add-on
- [custom_components/wakemeup_bridge](./custom_components/wakemeup_bridge) - optional Home Assistant bridge integration for stable Lovelace API routes
- [docs/wakemeup-bridge.md](./docs/wakemeup-bridge.md) - install and migration notes for the bridge and Lovelace card
- [LICENSE](./LICENSE) - repository license

## What WakeMeUp Does

- Manages multiple alarms
- Supports one-time and repeating alarms
- Supports bulk enable and disable actions for all alarms from the add-on UI
- Supports repeat modes:
  - Never
  - Daily
  - Weekdays
  - Weekends
  - Custom days
- Publishes a Home Assistant event when an alarm fires
- Stores alarms and app settings in SQLite
- Runs frontend and backend in a single container
- Supports light, dark, and automatic themes
- Supports persisted UI language selection

WakeMeUp does not perform the final wake-up automation itself. Home Assistant automations should react to the emitted event.

The add-on UI also includes bulk enable and disable actions for all alarms directly in the sidebar menu.

## Languages

WakeMeUp currently includes these UI languages:

- English
- German
- French
- Spanish
- Portuguese
- Italian
- Slovak
- Czech
- Polish
- Ukrainian
- Greek
- Esperanto
- Klingon

The selected language is stored in the add-on database and is also exposed through the JSON API for future external clients.

## Event

All alarms emit the same Home Assistant event:

`wakemeup_alarm_triggered`

Event payload includes:

- `name`
- `time`
- `description`

If an alarm has no description, WakeMeUp sends an empty string.

## Storage

WakeMeUp uses SQLite.

In Home Assistant add-on mode, the database is stored in:

`/data/wakemeup.db`

That location is persistent across add-on restarts and updates.

ASP.NET Data Protection keys are also persisted under `/data/.aspnet/DataProtection-Keys` so add-on restarts do not break antiforgery/session state.

## Home Assistant Add-on Notes

- Ingress is enabled
- Home Assistant API access is enabled
- The add-on uses the Supervisor-provided Home Assistant Core endpoint
- Event publishing uses the internal Home Assistant endpoint via Supervisor
- No user-facing add-on options are currently exposed in `config.yaml`

Repository URL:

`https://github.com/mholubek/WakeMeUp`

To add it manually in Home Assistant:

1. Open `Settings > Add-ons > Add-on Store`
2. Open the top-right menu
3. Choose `Repositories`
4. Add `https://github.com/mholubek/WakeMeUp`

## JSON API

WakeMeUp already exposes a JSON API intended for future clients.

Available endpoints:

- `GET /api/v1/alarms`
- `GET /api/v1/alarms/{id}`
- `POST /api/v1/alarms`
- `PUT /api/v1/alarms/{id}`
- `PATCH /api/v1/alarms/{id}/enabled`
- `DELETE /api/v1/alarms/{id}`
- `GET /api/v1/meta`
- `GET /health`

Alarm responses include:

- `id`
- `name`
- `description`
- `time`
- `isEnabled`
- `repeatMode`
- `days`
- `nextTriggerLocal`
- `createdUtc`
- `lastTriggeredUtc`
- `lastResultMessage`

Metadata responses include:

- `timeZoneId`
- `timeZoneDisplayName`
- `currentLanguage`
- `currentTheme`
- `currentLocalTime`
- `supportedLanguages`
- `repeatModes`
- `weekdays`

Validation failures return structured JSON error details so clients can surface field-specific errors.

## HACS Card Preparation

The backend is already prepared for a future custom HACS dashboard card.

Current assumptions for that card:

- It will run inside the Home Assistant dashboard
- It can use either the add-on JSON API directly or the stable Home Assistant bridge routes
- It should keep its own localized UI strings while reading the currently selected app language from `/api/wakemeup/meta` or `/api/v1/meta`

## WakeMeUp Bridge

This repository also includes an optional companion Home Assistant custom integration:

- [custom_components/wakemeup_bridge](./custom_components/wakemeup_bridge)

[![Open your Home Assistant instance and start the WakeMeUp Bridge setup flow](https://my.home-assistant.io/badges/config_flow_start.svg)](https://my.home-assistant.io/redirect/config_flow_start?domain=wakemeup_bridge)

The bridge exposes stable authenticated Home Assistant endpoints:

- `GET /api/wakemeup/meta`
- `GET /api/wakemeup/alarms`
- `POST /api/wakemeup/alarms`
- `PATCH /api/wakemeup/alarms/enabled`
- `GET /api/wakemeup/alarms/{id}`
- `PUT /api/wakemeup/alarms/{id}`
- `DELETE /api/wakemeup/alarms/{id}`
- `PATCH /api/wakemeup/alarms/{id}/enabled`

This lets a Lovelace card call WakeMeUp through the normal Home Assistant frontend origin without knowing the add-on ingress URL.
The bridge auto-detects the installed WakeMeUp add-on through Supervisor, resolves its internal hostname automatically, and proxies requests to the add-on API.

The bridge also exposes helper entities in Home Assistant:

- `binary_sensor.wakemeup_bridge_alarm_triggered` turns on for 30 seconds when an alarm fires and includes the last alarm details in its attributes
- `button.enable_all_alarms`
- `button.disable_all_alarms`

Entity names stay fixed in English by design so frontend and HACS components can localize their own UI independently.

Installation summary:

1. Copy [custom_components/wakemeup_bridge](./custom_components/wakemeup_bridge) into your Home Assistant `config/custom_components` directory.
2. Restart Home Assistant.
3. Add the `WakeMeUp Bridge` integration from `Settings > Devices & Services`.
4. Ensure the WakeMeUp add-on is installed and running.

See [custom_components/wakemeup_bridge/README.md](./custom_components/wakemeup_bridge/README.md) and [docs/wakemeup-bridge.md](./docs/wakemeup-bridge.md) for installation details and the minimal frontend migration steps.

## Add-on Folder

The add-on itself lives in [wakemeup](./wakemeup) and includes:

- `config.yaml`
- `Dockerfile`
- `DOCS.md`
- `CHANGELOG.md`
- `icon.png`
- `logo.png`
- the ASP.NET application source code

## Local Development

Build from the repository root:

```bash
dotnet build WakeMeUp.slnx
```

Or build the project directly:

```bash
dotnet build wakemeup/WakeMeUp.csproj
```

## License

This project uses the MIT License.

You can use it, modify it, share it, and use it commercially as long as the license notice is preserved.

See [LICENSE](./LICENSE) for details.

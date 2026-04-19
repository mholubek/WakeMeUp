# WakeMeUp

WakeMeUp is a Home Assistant alarm add-on built with ASP.NET 10 Blazor Server.

It allows users to create alarms with repeat rules and emits the `wakemeup_alarm_triggered` event when an alarm becomes due.

## What this add-on does

- Manage multiple alarms
- Support one-time and repeating alarms
- Support repeat modes:
  - Never
  - Daily
  - Weekdays
  - Weekends
  - Custom days
- Emit a Home Assistant event when an alarm fires
- Keep alarm data and settings in SQLite
- Run frontend and backend in one container
- Support light, dark, and automatic themes
- Support persisted UI language selection

WakeMeUp does not perform the final wake-up automation itself. Home Assistant automations should react to the emitted event.

## Included Languages

WakeMeUp currently ships with these UI languages:

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

## Event

All alarms emit the same Home Assistant event:

`wakemeup_alarm_triggered`

Event payload includes:

- `name`
- `time`
- `description`

If an alarm has no description, WakeMeUp sends an empty string.

## Storage

In Home Assistant add-on mode, SQLite data is stored in:

`/data/wakemeup.db`

This location is persistent across add-on restarts and updates.

ASP.NET Data Protection keys are persisted under:

`/data/.aspnet/DataProtection-Keys`

## Configuration

This add-on currently does not expose user-configurable add-on options in `config.yaml`.

Home Assistant API access is handled through the Supervisor environment and the internal Home Assistant endpoint.

## JSON API

WakeMeUp exposes a JSON API intended for future clients such as a custom HACS card.

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

Validation failures return structured JSON error details.

## HACS Card Preparation

The backend is already prepared for a future custom HACS dashboard card.

The expected model is:

- the card runs inside the Home Assistant dashboard
- the card reaches WakeMeUp through the same Home Assistant ingress base
- the card uses the JSON API for alarm CRUD and enable/disable actions
- the card reads the selected app language from `/api/v1/meta`
- the card keeps its own localized UI strings

## Notes

- Ingress is enabled
- Home Assistant API access is required
- The app uses the Home Assistant internal Core endpoint through Supervisor

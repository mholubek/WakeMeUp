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

## Configuration

This add-on currently does not expose user-configurable add-on options in `config.yaml`.

Home Assistant API access is handled through the Supervisor environment and the internal Home Assistant endpoint.

## Notes

- Ingress is enabled
- Home Assistant API access is required
- The app uses the Home Assistant internal Core endpoint through Supervisor

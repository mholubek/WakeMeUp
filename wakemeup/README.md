# WakeMeUp Add-on

WakeMeUp is a Home Assistant alarm add-on that lets users create alarms with repeat rules and emits the `wakemeup_alarm_triggered` event when an alarm becomes due.

The add-on runs as a single ASP.NET 10 Blazor Server application and stores its data in SQLite.

## Features

- Multiple alarms
- One-time and repeating alarms
- Repeat modes:
  - Never
  - Daily
  - Weekdays
  - Weekends
  - Custom days
- Home Assistant event publishing
- SQLite persistence
- Light, dark, and automatic themes
- Persisted UI language selection
- JSON API for future clients such as a custom HACS card

## Included Languages

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

All alarms emit:

`wakemeup_alarm_triggered`

Payload fields:

- `name`
- `time`
- `description`

See [DOCS.md](./DOCS.md) for add-on usage details.

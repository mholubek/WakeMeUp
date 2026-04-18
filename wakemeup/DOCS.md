# WakeMeUp

WakeMeUp is a Home Assistant alarm add-on built with ASP.NET 10 Blazor Server.

It allows users to create alarms with repeat rules and emits the `wakemeup_alarm_triggered` event when an alarm becomes due.

## What this add-on does

- Manage multiple alarms
- Support one-time and repeating alarms
- Emit a Home Assistant event when an alarm fires
- Keep alarm data in SQLite
- Run frontend and backend in one container

WakeMeUp does not perform the final wake-up automation itself. Home Assistant automations should react to the emitted event.

## Event

All alarms emit the same Home Assistant event:

`wakemeup_alarm_triggered`

Event payload includes:

- `alarmId`
- `alarmName`
- `description`
- `repeatMode`
- `scheduledLocal`
- `triggeredUtc`

## Storage

In Home Assistant add-on mode, SQLite data is stored in:

`/data/wakemeup.db`

This location is persistent across add-on restarts and updates.

## Configuration

This add-on currently does not expose user-configurable add-on options in `config.yaml`.

Home Assistant API access is handled through the Supervisor environment and `SUPERVISOR_TOKEN`.

## Notes

- Ingress is enabled
- Home Assistant API access is required
- The app uses the Home Assistant internal Core endpoint through Supervisor

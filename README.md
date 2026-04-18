# WakeMeUp

WakeMeUp is a ASP.NET 10 Blazor Server alarm application built for Home Assistant.

The app lets users create multiple alarms, define repeat rules, and emit a Home Assistant event when an alarm fires. WakeMeUp does not perform the final automation itself. It only publishes the `wakemeup_alarm_triggered` event, which can then be handled by Home Assistant automations.

This project is fully vibe coded.

## Features

- Create, edit, enable, disable, and delete multiple alarms
- Support one-time and repeating alarms
- Repeat options:
  - Never
  - Daily
  - Weekdays
  - Weekends
  - Custom days
- Background scheduler that continuously checks for due alarms
- Home Assistant event publishing through the Supervisor/Core API
- Light, dark, and automatic theme support
- Monolithic deployment: backend and frontend in a single container
- SQLite persistence

## Tech Stack

- .NET 10
- ASP.NET Core Blazor Server
- Bootstrap
- SQLite

## How It Works

When an alarm becomes due, WakeMeUp sends the following Home Assistant event:

`wakemeup_alarm_triggered`

The payload includes alarm-related metadata such as:

- `alarmId`
- `alarmName`
- `description`
- `repeatMode`
- `scheduledLocal`
- `triggeredUtc`

This makes it possible to build Home Assistant automations that react to the event and perform the actual wake-up behavior.

## Persistence

WakeMeUp stores application data in SQLite.

- In Home Assistant add-on environments, the database is stored in `/data/wakemeup.db`
- Outside add-on environments, the fallback location is `App_Data/wakemeup.db`

## Home Assistant Integration

WakeMeUp is designed to run as a Home Assistant add-on.

It uses:

- `http://supervisor/core` as the internal Home Assistant Core endpoint
- `SUPERVISOR_TOKEN` from the add-on environment for authentication

The user does not need to enter the Home Assistant URL or token in the app UI.

## Project Structure

- [Program.cs](./Program.cs) - app startup and dependency registration
- [Components](./Components) - Blazor UI
- [Services](./Services) - scheduler, event publishing, persistence
- [Domain](./Domain) - alarm and settings models
- [wwwroot](./wwwroot) - static assets and styles
- [config.yaml](./config.yaml) - Home Assistant add-on configuration
- [repository.yaml](./repository.yaml) - Home Assistant add-on repository metadata
- [DOCS.md](./DOCS.md) - add-on documentation shown to users
- [CHANGELOG.md](./CHANGELOG.md) - add-on release notes

## Running Locally

Requirements:

- .NET 10 SDK

Run:

```bash
dotnet build
dotnet run
```

Open the app in your browser using the URL shown by ASP.NET Core.

## Docker

The repository includes a `Dockerfile`.

Build:

```bash
docker build -t wakemeup .
```

Run:

```bash
docker run -p 8080:8080 wakemeup
```

For persistent SQLite storage outside Home Assistant, mount a volume and optionally set `WAKEMEUP_DB_PATH`.

## Home Assistant Add-on Notes

The included `config.yaml` is a starting point for add-on packaging.

Current add-on behavior assumes:

- Ingress is enabled
- Home Assistant API access is enabled
- The add-on receives `SUPERVISOR_TOKEN`
- The repository contains `repository.yaml` so it can act as a Home Assistant add-on repository

## Current Event Model

All alarms emit the same Home Assistant event:

`wakemeup_alarm_triggered`

Home Assistant does not distinguish alarms by event name. If needed, automations can inspect the event payload.

## License

This project uses the PolyForm Noncommercial License 1.0.0.

You can use it, modify it, and share it for non-commercial purposes, but selling it or using it commercially is not allowed.

See [LICENSE](./LICENSE) for details.

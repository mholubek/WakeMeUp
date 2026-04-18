# WakeMeUp Add-on Repository

This repository contains the Home Assistant add-on for WakeMeUp.

WakeMeUp is an alarm application built with ASP.NET 10 Blazor Server. It allows users to create alarms with repeat rules and emits the `wakemeup_alarm_triggered` event when an alarm becomes due.

This project is fully vibe coded.

[![Open your Home Assistant instance and add this repository](https://my.home-assistant.io/badges/supervisor_add_addon_repository.svg)](https://my.home-assistant.io/redirect/supervisor_add_addon_repository/?repository_url=https%3A%2F%2Fgithub.com%2Fmholubek%2FWakeMeUp)
[![Build WakeMeUp Add-on](https://github.com/mholubek/WakeMeUp/actions/workflows/build-addon.yml/badge.svg)](https://github.com/mholubek/WakeMeUp/actions/workflows/build-addon.yml)

## Repository Layout

- [repository.yaml](./repository.yaml) - Home Assistant add-on repository metadata
- [wakemeup](./wakemeup) - the WakeMeUp add-on
- [LICENSE](./LICENSE) - repository license

## Add-on Folder

The add-on itself lives in [wakemeup](./wakemeup) and includes:

- `config.yaml`
- `Dockerfile`
- `DOCS.md`
- `CHANGELOG.md`
- `icon.png`
- `logo.png`
- the ASP.NET application source code

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
- Frontend and backend in a single container

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

## Local Development

Build from the repository root:

```bash
dotnet build WakeMeUp.slnx
```

Or build the project directly:

```bash
dotnet build wakemeup/WakeMeUp.csproj
```

## Home Assistant

This repository is prepared to act as a Home Assistant add-on repository.

Repository URL:

`https://github.com/mholubek/WakeMeUp`

The add-on metadata is defined in [repository.yaml](./repository.yaml), while the actual add-on files are stored in [wakemeup](./wakemeup).

To add it manually in Home Assistant:

1. Open `Settings > Add-ons > Add-on Store`
2. Open the top-right menu
3. Choose `Repositories`
4. Add `https://github.com/mholubek/WakeMeUp`

## License

This project uses the PolyForm Noncommercial License 1.0.0.

You can use it, modify it, and share it for non-commercial purposes, but selling it or using it commercially is not allowed.

See [LICENSE](./LICENSE) for details.

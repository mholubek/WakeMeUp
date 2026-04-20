# WakeMeUp Card

A bubble-inspired Home Assistant custom card for WakeMeUp alarms via the Home Assistant backend bridge.
This card is full vibe coded.

## Features

- List alarms
- Create alarms
- Edit alarms
- Delete alarms
- Toggle alarm enabled state
- Bulk enable all alarms
- Bulk disable all alarms
- Show API validation errors
- Load UI language from `GET /api/wakemeup/meta` with English fallback
- Communicate only through Home Assistant bridge endpoints under `/api/wakemeup/*`

## Install

1. Copy `dist/wakemeup-card.js` to your Home Assistant `www` folder or publish this repository through HACS.
2. Add the resource:

```yaml
url: /hacsfiles/wakemeup-card/wakemeup-card.js
type: module
```

## Lovelace Example

```yaml
type: custom:wakemeup-card
title: WakeMeUp
```

## Configuration

- `title`: Optional card title.

No base URL, ingress URL, hostname, or port configuration is required.

## Development

```bash
npm install
npm run typecheck
npm run build
```

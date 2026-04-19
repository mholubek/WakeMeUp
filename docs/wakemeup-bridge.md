# WakeMeUp Bridge

`wakemeup_bridge` is a companion Home Assistant custom integration for the WakeMeUp add-on.

Its purpose is to expose stable Home Assistant API endpoints for a Lovelace card so the frontend does not need to know or configure the add-on ingress URL.

## What it does

The bridge registers authenticated Home Assistant endpoints under:

- `GET /api/wakemeup/meta`
- `GET /api/wakemeup/alarms`
- `POST /api/wakemeup/alarms`
- `GET /api/wakemeup/alarms/{id}`
- `PUT /api/wakemeup/alarms/{id}`
- `DELETE /api/wakemeup/alarms/{id}`
- `PATCH /api/wakemeup/alarms/{id}/enabled`

Internally, the bridge:

1. Uses the Supervisor API to resolve the installed WakeMeUp add-on repository identifier and hostname.
2. Calls the existing WakeMeUp add-on API over the internal Home Assistant network.
3. Returns the same JSON payloads to the Lovelace card.

The card only needs to call `/api/wakemeup/...` on the Home Assistant origin.

## Installation

1. Copy the folder [custom_components/wakemeup_bridge](../custom_components/wakemeup_bridge) into your Home Assistant config directory under:
   - `config/custom_components/wakemeup_bridge`
2. Restart Home Assistant.
3. Open `Settings > Devices & Services`.
4. Click `Add Integration`.
5. Search for `WakeMeUp Bridge`.
6. Create the single bridge entry.
7. Make sure the WakeMeUp add-on is installed and started.

After that, the Home Assistant backend will expose the stable `/api/wakemeup/...` routes for the card.

## Minimal Lovelace Card Migration

If the current card uses a configurable ingress base path, the minimal migration is:

1. Remove the ingress/base path configuration from the card config.
2. Replace the API base URL with:

```ts
const API_BASE = "/api/wakemeup";
```

3. Keep the existing request payloads and response parsing.

Example endpoint mapping:

- `GET ${API_BASE}/meta`
- `GET ${API_BASE}/alarms`
- `POST ${API_BASE}/alarms`
- `GET ${API_BASE}/alarms/${id}`
- `PUT ${API_BASE}/alarms/${id}`
- `DELETE ${API_BASE}/alarms/${id}`
- `PATCH ${API_BASE}/alarms/${id}/enabled`

## Frontend Simplification

With the bridge in place, the card no longer needs to:

- know the add-on ingress path
- store or prompt for a base path
- handle direct add-on URL construction
- implement its own add-on discovery logic

The Home Assistant session and backend handle access through the normal authenticated frontend context.

## Language Handling

The card should continue to read the selected app language from:

- `GET /api/wakemeup/meta`

The card can then use its own internal translations and switch UI text based on the returned `currentLanguage` value.

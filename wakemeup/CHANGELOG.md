# Changelog

## 0.2.2

- Moved the add-on bulk enable and disable flow into a dedicated `BulkAlarmToggleService` while keeping the existing UI behavior
- Added localized app UI strings for bulk enable and disable actions across all supported WakeMeUp languages
- Fixed WakeMeUp Bridge helper entity names to stay permanently in English
- Added Python unit tests for the WakeMeUp Bridge bulk endpoint and helper entities

## 0.2.1

- Added bulk enable and disable actions directly in the add-on UI sidebar
- Added shared app state wiring so bulk alarm actions can be triggered from the navigation and applied on the overview page
- Added batch SQLite alarm saving to persist bulk enable and disable operations efficiently

## 0.2.0

- Added WakeMeUp Bridge helper entities for Home Assistant automations: a temporary `alarm_triggered` binary sensor plus bulk enable and disable buttons for all alarms
- Added the bridge bulk API endpoint `PATCH /api/wakemeup/alarms/enabled` for companion app and HACS card control

## 0.1.9

- Auto-detect the initial app language for new users from Home Assistant first, then from the browser, with English as the fallback
- Added the optional `wakemeup_bridge` Home Assistant custom integration to expose stable `/api/wakemeup/...` endpoints for Lovelace cards
- Added bridge installation and Lovelace migration documentation

## 0.1.8

- Simplified the alarm enable switch on desktop by removing its frame and duplicate label
- Increased the alarm enable switch size on desktop and mobile layouts
- Moved the mobile alarm enable switch into the same row as the alarm time
- Updated next trigger formatting to follow the selected language and culture

## 0.1.7

- Added a versioned JSON API for alarms, metadata, and quick enable or disable actions
- Added shared alarm mutation and validation logic for both the Blazor UI and future external clients
- Prepared the backend for a future custom HACS card through ingress-friendly API endpoints
- Updated repository and add-on documentation to cover languages, API endpoints, storage, and HACS card preparation

## 0.1.6

- Added full UI language selection with persisted app language settings
- Added German, French, Spanish, Portuguese, Italian, Slovak, Czech, Polish, Ukrainian, Greek, Esperanto, and Klingon UI translations
- Applied the selected language to UI text and culture-aware date formatting
- Improved mobile alarm card and editor layout
- Added icons to main action buttons for alarm cards and the editor
- Updated toast messages to dismiss on click and automatically disappear after 5 seconds

## 0.1.5

- Removed the remaining fallback text for blank alarm descriptions from the UI and custom logs
- Added explicit timestamps to custom WakeMeUp alarm logs

## 0.1.4

- Sent an empty string when an alarm description is blank
- Fixed one-time alarms so re-enabling them resets their processed state and allows them to trigger again

## 0.1.3

- Added direct alarm enable and disable switches to the overview cards with desktop and mobile specific placement
- Improved alarm card layout for long alarm names
- Added clearer alarm set, triggered, deleted, and error logs in Home Assistant
- Simplified the Home Assistant event payload to `name`, `time`, and `description`

## 0.1.2

- Persisted ASP.NET Data Protection keys in the add-on data directory to avoid antiforgery token issues after restarts
- Disabled HTTPS redirection inside the Home Assistant add-on environment to remove ingress-related warnings

## 0.1.1

- Fixed Home Assistant ingress asset loading so styles and interactivity work correctly in the add-on UI
- Refreshed add-on icon and logo artwork
- Fixed add-on Docker build compatibility with stable .NET 10 images

## 0.1.0

- Initial Home Assistant add-on packaging
- Alarm overview, editor modal, and settings page
- SQLite persistence
- Home Assistant event publishing with `wakemeup_alarm_triggered`

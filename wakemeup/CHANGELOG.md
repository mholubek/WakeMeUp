# Changelog

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

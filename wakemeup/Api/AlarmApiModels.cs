namespace WakeMeUp.Api;

public sealed record AlarmResponse(
    Guid Id,
    string Name,
    string Description,
    string Time,
    bool IsEnabled,
    string RepeatMode,
    IReadOnlyList<string> Days,
    string? NextTriggerLocal,
    string CreatedUtc,
    string? LastTriggeredUtc,
    string LastResultMessage);

public sealed record AlarmUpsertRequest(
    string? Name,
    string? Description,
    string? Time,
    bool IsEnabled,
    string? RepeatMode,
    IReadOnlyList<string>? Days);

public sealed record AlarmToggleRequest(bool IsEnabled);

public sealed record ApiOptionResponse(string Value, string Label);

public sealed record AlarmApiMetadataResponse(
    string TimeZoneId,
    string TimeZoneDisplayName,
    string CurrentLanguage,
    string CurrentTheme,
    string CurrentLocalTime,
    IReadOnlyList<ApiOptionResponse> SupportedLanguages,
    IReadOnlyList<ApiOptionResponse> RepeatModes,
    IReadOnlyList<ApiOptionResponse> Weekdays);

public sealed record ApiValidationErrorResponse(string Field, string Code, string Message);

public sealed record ApiErrorResponse(string Error, IReadOnlyList<ApiValidationErrorResponse> Details);

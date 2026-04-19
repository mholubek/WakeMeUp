using WakeMeUp.Domain;

namespace WakeMeUp.Services;

public sealed class AlarmMutationService
{
    public bool TryBuild(
        AlarmMutationInput input,
        AlarmDefinition? existingAlarm,
        out AlarmDefinition alarm,
        out AlarmValidationError? validationError)
    {
        validationError = Validate(input);
        if (validationError is not null)
        {
            alarm = new AlarmDefinition();
            return false;
        }

        _ = TimeOnly.TryParse(input.TimeText, out var time);
        var description = input.Description?.Trim() ?? string.Empty;
        var days = input.RepeatMode == RepeatMode.CustomDays
            ? input.Days
            : WeekdaySelection.None;

        alarm = new AlarmDefinition
        {
            Id = existingAlarm?.Id ?? input.Id,
            Name = input.Name!.Trim(),
            Description = description,
            Time = time,
            IsEnabled = input.IsEnabled,
            RepeatMode = input.RepeatMode,
            Days = days,
            CreatedUtc = existingAlarm?.CreatedUtc ?? DateTimeOffset.UtcNow,
            LastProcessedOccurrenceUtc = existingAlarm?.LastProcessedOccurrenceUtc,
            LastTriggeredUtc = existingAlarm?.LastTriggeredUtc,
            LastResultMessage = existingAlarm?.LastResultMessage ?? string.Empty
        };

        if (existingAlarm is not null &&
            !existingAlarm.IsEnabled &&
            alarm.IsEnabled &&
            alarm.RepeatMode == RepeatMode.Never)
        {
            alarm.LastProcessedOccurrenceUtc = null;
            alarm.LastTriggeredUtc = null;
            alarm.LastResultMessage = string.Empty;
        }

        return true;
    }

    public AlarmDefinition ApplyEnabledState(AlarmDefinition existingAlarm, bool isEnabled)
    {
        var updatedAlarm = Clone(existingAlarm);
        updatedAlarm.IsEnabled = isEnabled;

        if (!existingAlarm.IsEnabled && isEnabled && existingAlarm.RepeatMode == RepeatMode.Never)
        {
            updatedAlarm.LastProcessedOccurrenceUtc = null;
            updatedAlarm.LastTriggeredUtc = null;
            updatedAlarm.LastResultMessage = string.Empty;
        }

        return updatedAlarm;
    }

    public AlarmDefinition Clone(AlarmDefinition source)
    {
        return new AlarmDefinition
        {
            Id = source.Id,
            Name = source.Name,
            Description = source.Description,
            Time = source.Time,
            IsEnabled = source.IsEnabled,
            RepeatMode = source.RepeatMode,
            Days = source.Days,
            CreatedUtc = source.CreatedUtc,
            LastProcessedOccurrenceUtc = source.LastProcessedOccurrenceUtc,
            LastTriggeredUtc = source.LastTriggeredUtc,
            LastResultMessage = source.LastResultMessage
        };
    }

    private static AlarmValidationError? Validate(AlarmMutationInput input)
    {
        if (string.IsNullOrWhiteSpace(input.Name))
        {
            return new AlarmValidationError("name", "name_required", "Name is required.");
        }

        if (input.Name.Trim().Length > 80)
        {
            return new AlarmValidationError("name", "name_too_long", "Name must be 80 characters or fewer.");
        }

        if (!TimeOnly.TryParse(input.TimeText, out _))
        {
            return new AlarmValidationError("time", "time_format_invalid", "Time must use the HH:mm format.");
        }

        if ((input.Description?.Trim().Length ?? 0) > 160)
        {
            return new AlarmValidationError("description", "description_too_long", "Note must be 160 characters or fewer.");
        }

        if (input.RepeatMode == RepeatMode.CustomDays && input.Days == WeekdaySelection.None)
        {
            return new AlarmValidationError("days", "custom_days_required", "Select at least one day for a custom repeat rule.");
        }

        return null;
    }
}

public sealed record AlarmMutationInput(
    Guid Id,
    string? Name,
    string? Description,
    string? TimeText,
    bool IsEnabled,
    RepeatMode RepeatMode,
    WeekdaySelection Days);

public sealed record AlarmValidationError(string Field, string Code, string Message);

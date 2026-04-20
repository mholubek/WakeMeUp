using WakeMeUp.Domain;
using WakeMeUp.Services;

namespace WakeMeUp.Tests;

public sealed class AlarmMutationServiceTests
{
    private readonly AlarmMutationService _service = new();

    [Fact]
    public void TryBuild_ReturnsValidationError_ForCustomDaysWithoutSelection()
    {
        var input = new AlarmMutationInput(
            Guid.NewGuid(),
            "Morning",
            string.Empty,
            "07:00",
            true,
            RepeatMode.CustomDays,
            WeekdaySelection.None);

        var success = _service.TryBuild(input, existingAlarm: null, out _, out var validationError);

        Assert.False(success);
        Assert.NotNull(validationError);
        Assert.Equal("days", validationError.Field);
        Assert.Equal("custom_days_required", validationError.Code);
    }

    [Fact]
    public void TryBuild_TrimsNameAndClearsNonCustomDaysSelection()
    {
        var input = new AlarmMutationInput(
            Guid.NewGuid(),
            "  Morning  ",
            "  Hello  ",
            "07:00",
            true,
            RepeatMode.Daily,
            WeekdaySelection.Monday | WeekdaySelection.Tuesday);

        var success = _service.TryBuild(input, existingAlarm: null, out var alarm, out var validationError);

        Assert.True(success);
        Assert.Null(validationError);
        Assert.Equal("Morning", alarm.Name);
        Assert.Equal("Hello", alarm.Description);
        Assert.Equal(WeekdaySelection.None, alarm.Days);
    }

    [Fact]
    public void ApplyEnabledState_ResetsOneTimeAlarmHistory_WhenReEnabled()
    {
        var existing = new AlarmDefinition
        {
            Name = "One shot",
            IsEnabled = false,
            RepeatMode = RepeatMode.Never,
            LastProcessedOccurrenceUtc = DateTimeOffset.UtcNow,
            LastTriggeredUtc = DateTimeOffset.UtcNow,
            LastResultMessage = "Event sent."
        };

        var updated = _service.ApplyEnabledState(existing, isEnabled: true);

        Assert.True(updated.IsEnabled);
        Assert.Null(updated.LastProcessedOccurrenceUtc);
        Assert.Null(updated.LastTriggeredUtc);
        Assert.Equal(string.Empty, updated.LastResultMessage);
    }
}

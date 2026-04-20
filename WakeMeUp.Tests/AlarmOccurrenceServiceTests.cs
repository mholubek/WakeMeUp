using WakeMeUp.Domain;
using WakeMeUp.Services;

namespace WakeMeUp.Tests;

public sealed class AlarmOccurrenceServiceTests
{
    private readonly AlarmOccurrenceService _service = new();

    [Fact]
    public void GetNextOccurrence_ReturnsNull_WhenAlarmIsDisabled()
    {
        var alarm = CreateAlarm(isEnabled: false);

        var next = _service.GetNextOccurrence(alarm, new DateTimeOffset(2026, 4, 20, 6, 0, 0, TimeSpan.FromHours(2)));

        Assert.Null(next);
    }

    [Fact]
    public void GetNextOccurrence_ReturnsNextWeekday_ForWeekdayAlarmAfterTodayTime()
    {
        var alarm = CreateAlarm(repeatMode: RepeatMode.Weekdays, time: new TimeOnly(6, 0));
        var now = new DateTimeOffset(2026, 4, 24, 7, 0, 0, TimeSpan.FromHours(2)); // Friday

        var next = _service.GetNextOccurrence(alarm, now);

        Assert.Equal(new DateTimeOffset(2026, 4, 27, 6, 0, 0, TimeSpan.FromHours(2)), next);
    }

    [Fact]
    public void GetDueOccurrence_ReturnsNull_WhenOccurrenceWasAlreadyProcessed()
    {
        var dueUtc = new DateTimeOffset(2026, 4, 20, 6, 0, 0, TimeSpan.FromHours(2)).ToUniversalTime();
        var alarm = CreateAlarm(
            repeatMode: RepeatMode.Daily,
            time: new TimeOnly(6, 0),
            lastProcessedOccurrenceUtc: dueUtc);
        var now = new DateTimeOffset(2026, 4, 20, 6, 0, 30, TimeSpan.FromHours(2));

        var due = _service.GetDueOccurrence(alarm, now);

        Assert.Null(due);
    }

    [Fact]
    public void GetDueOccurrence_ReturnsOccurrence_WhenAlarmIsWithinOneMinuteWindow()
    {
        var alarm = CreateAlarm(repeatMode: RepeatMode.Daily, time: new TimeOnly(6, 0));
        var now = new DateTimeOffset(2026, 4, 20, 6, 0, 30, TimeSpan.FromHours(2));

        var due = _service.GetDueOccurrence(alarm, now);

        Assert.Equal(new DateTimeOffset(2026, 4, 20, 6, 0, 0, TimeSpan.FromHours(2)), due);
    }

    private static AlarmDefinition CreateAlarm(
        bool isEnabled = true,
        RepeatMode repeatMode = RepeatMode.Never,
        TimeOnly? time = null,
        WeekdaySelection days = WeekdaySelection.None,
        DateTimeOffset? lastProcessedOccurrenceUtc = null)
    {
        return new AlarmDefinition
        {
            Name = "Test alarm",
            Description = string.Empty,
            IsEnabled = isEnabled,
            RepeatMode = repeatMode,
            Time = time ?? new TimeOnly(7, 0),
            Days = days,
            LastProcessedOccurrenceUtc = lastProcessedOccurrenceUtc
        };
    }
}

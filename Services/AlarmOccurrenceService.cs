using WakeMeUp.Domain;

namespace WakeMeUp.Services;

public sealed class AlarmOccurrenceService
{
    public DateTimeOffset? GetNextOccurrence(AlarmDefinition alarm, DateTimeOffset now)
    {
        if (!alarm.IsEnabled)
        {
            return null;
        }

        var localNow = now.ToLocalTime();

        for (var dayOffset = 0; dayOffset < 14; dayOffset++)
        {
            var date = DateOnly.FromDateTime(localNow.Date.AddDays(dayOffset));

            if (!IsDateEligible(alarm, date))
            {
                continue;
            }

            var candidateLocal = date.ToDateTime(alarm.Time, DateTimeKind.Local);
            var candidate = new DateTimeOffset(candidateLocal);

            if (candidate <= localNow)
            {
                continue;
            }

            if (alarm.RepeatMode == RepeatMode.Never && alarm.LastProcessedOccurrenceUtc.HasValue)
            {
                return null;
            }

            return candidate;
        }

        return null;
    }

    public DateTimeOffset? GetDueOccurrence(AlarmDefinition alarm, DateTimeOffset now)
    {
        if (!alarm.IsEnabled)
        {
            return null;
        }

        var localNow = now.ToLocalTime();

        for (var dayOffset = 0; dayOffset < 2; dayOffset++)
        {
            var date = DateOnly.FromDateTime(localNow.Date.AddDays(-dayOffset));

            if (!IsDateEligible(alarm, date))
            {
                continue;
            }

            var candidateLocal = date.ToDateTime(alarm.Time, DateTimeKind.Local);
            var candidate = new DateTimeOffset(candidateLocal);

            if (candidate > localNow)
            {
                continue;
            }

            if (localNow - candidate > TimeSpan.FromMinutes(1))
            {
                continue;
            }

            if (alarm.LastProcessedOccurrenceUtc.HasValue &&
                alarm.LastProcessedOccurrenceUtc.Value.UtcDateTime == candidate.UtcDateTime)
            {
                return null;
            }

            if (alarm.RepeatMode == RepeatMode.Never && alarm.LastProcessedOccurrenceUtc.HasValue)
            {
                return null;
            }

            return candidate;
        }

        return null;
    }

    private static bool IsDateEligible(AlarmDefinition alarm, DateOnly date)
    {
        return alarm.RepeatMode switch
        {
            RepeatMode.Never => true,
            RepeatMode.Daily => true,
            RepeatMode.Weekdays => IsWeekday(date.DayOfWeek),
            RepeatMode.Weekends => !IsWeekday(date.DayOfWeek),
            RepeatMode.CustomDays => MatchesSelectedDay(date.DayOfWeek, alarm.Days),
            _ => false
        };
    }

    private static bool IsWeekday(DayOfWeek dayOfWeek)
    {
        return dayOfWeek is >= DayOfWeek.Monday and <= DayOfWeek.Friday;
    }

    private static bool MatchesSelectedDay(DayOfWeek dayOfWeek, WeekdaySelection days)
    {
        var selection = dayOfWeek switch
        {
            DayOfWeek.Monday => WeekdaySelection.Monday,
            DayOfWeek.Tuesday => WeekdaySelection.Tuesday,
            DayOfWeek.Wednesday => WeekdaySelection.Wednesday,
            DayOfWeek.Thursday => WeekdaySelection.Thursday,
            DayOfWeek.Friday => WeekdaySelection.Friday,
            DayOfWeek.Saturday => WeekdaySelection.Saturday,
            DayOfWeek.Sunday => WeekdaySelection.Sunday,
            _ => WeekdaySelection.None
        };

        return selection != WeekdaySelection.None && days.HasFlag(selection);
    }
}

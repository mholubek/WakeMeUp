using WakeMeUp.Domain;

namespace WakeMeUp.Services;

public sealed class BulkAlarmToggleService(
    IAlarmStore store,
    AlarmMutationService alarmMutations,
    ILogger<BulkAlarmToggleService> logger)
{
    public async Task<BulkAlarmToggleResult> SetAllAlarmsEnabledAsync(
        bool isEnabled,
        CancellationToken cancellationToken = default)
    {
        var alarms = await store.GetAlarmsAsync(cancellationToken);
        return await SetAllAlarmsEnabledAsync(alarms, isEnabled, cancellationToken);
    }

    public async Task<BulkAlarmToggleResult> SetAllAlarmsEnabledAsync(
        IEnumerable<AlarmDefinition> alarms,
        bool isEnabled,
        CancellationToken cancellationToken = default)
    {
        var alarmsToUpdate = alarms
            .Where(alarm => alarm.IsEnabled != isEnabled)
            .Select(alarm => alarmMutations.ApplyEnabledState(alarm, isEnabled))
            .ToList();

        if (alarmsToUpdate.Count == 0)
        {
            return new BulkAlarmToggleResult(0, []);
        }

        await store.SaveAlarmsAsync(alarmsToUpdate, cancellationToken);

        foreach (var updatedAlarm in alarmsToUpdate)
        {
            logger.LogInformation(
                "[{LoggedAt}] Alarm updated: Name='{AlarmName}', Time='{AlarmTime}', Description='{AlarmDescription}', Enabled={IsEnabled}",
                GetLogTimestamp(),
                updatedAlarm.Name,
                updatedAlarm.Time.ToString("HH\\:mm"),
                GetLogDescription(updatedAlarm.Description),
                updatedAlarm.IsEnabled);
        }

        return new BulkAlarmToggleResult(alarmsToUpdate.Count, alarmsToUpdate);
    }

    private static string GetLogDescription(string? description)
    {
        return description?.Trim() ?? string.Empty;
    }

    private static string GetLogTimestamp()
    {
        return DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss zzz");
    }
}

public sealed record BulkAlarmToggleResult(int UpdatedCount, IReadOnlyList<AlarmDefinition> UpdatedAlarms);

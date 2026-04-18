using WakeMeUp.Domain;

namespace WakeMeUp.Services;

public sealed class AlarmScheduler(
    IServiceProvider serviceProvider,
    ILogger<AlarmScheduler> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(15));

        logger.LogInformation("WakeMeUp scheduler started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CheckAlarmsAsync(stoppingToken);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Unexpected error while checking alarms.");
            }

            await timer.WaitForNextTickAsync(stoppingToken);
        }
    }

    private async Task CheckAlarmsAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var store = scope.ServiceProvider.GetRequiredService<IAlarmStore>();
        var occurrenceService = scope.ServiceProvider.GetRequiredService<AlarmOccurrenceService>();
        var publisher = scope.ServiceProvider.GetRequiredService<HomeAssistantEventPublisher>();
        var alarms = await store.GetAlarmsAsync(cancellationToken);
        var now = DateTimeOffset.Now;

        foreach (var alarm in alarms)
        {
            var dueOccurrence = occurrenceService.GetDueOccurrence(alarm, now);

            if (!dueOccurrence.HasValue)
            {
                continue;
            }

            var (success, message) = await publisher.PublishAlarmTriggeredAsync(alarm, dueOccurrence.Value, cancellationToken);

            alarm.LastProcessedOccurrenceUtc = dueOccurrence.Value.ToUniversalTime();
            alarm.LastTriggeredUtc = success ? DateTimeOffset.UtcNow : alarm.LastTriggeredUtc;
            alarm.LastResultMessage = message;

            if (alarm.RepeatMode == RepeatMode.Never)
            {
                alarm.IsEnabled = false;
            }

            await store.SaveAlarmAsync(alarm, cancellationToken);

            if (!success)
            {
                logger.LogError(
                    "[{LoggedAt}] Error processing alarm: Name='{AlarmName}', Time='{AlarmTime}', Description='{AlarmDescription}', Result='{Result}'",
                    DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss zzz"),
                    alarm.Name,
                    alarm.Time.ToString("HH\\:mm"),
                    alarm.Description?.Trim() ?? string.Empty,
                    message);
            }
        }
    }
}

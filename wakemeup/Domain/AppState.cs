namespace WakeMeUp.Domain;

public sealed class AppState
{
    public List<AlarmDefinition> Alarms { get; set; } = [];
    public AppSettings Settings { get; set; } = new();

    public event Func<bool, Task>? BulkAlarmToggleRequested;

    public Task RequestBulkAlarmToggleAsync(bool isEnabled)
    {
        return BulkAlarmToggleRequested?.Invoke(isEnabled) ?? Task.CompletedTask;
    }
}

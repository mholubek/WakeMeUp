namespace WakeMeUp.Domain;

public sealed class AlarmDefinition
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "New alarm";
    public string Description { get; set; } = string.Empty;
    public TimeOnly Time { get; set; } = new(7, 0);
    public bool IsEnabled { get; set; } = true;
    public RepeatMode RepeatMode { get; set; } = RepeatMode.Never;
    public WeekdaySelection Days { get; set; } = WeekdaySelection.None;
    public DateTimeOffset CreatedUtc { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? LastProcessedOccurrenceUtc { get; set; }
    public DateTimeOffset? LastTriggeredUtc { get; set; }
    public string LastResultMessage { get; set; } = string.Empty;
}

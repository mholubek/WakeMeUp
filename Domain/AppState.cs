namespace WakeMeUp.Domain;

public sealed class AppState
{
    public List<AlarmDefinition> Alarms { get; set; } = [];
    public AppSettings Settings { get; set; } = new();
}

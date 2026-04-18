using WakeMeUp.Domain;

namespace WakeMeUp.Services;

public interface IAlarmStore
{
    Task<AppState> GetStateAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AlarmDefinition>> GetAlarmsAsync(CancellationToken cancellationToken = default);
    Task<AppSettings> GetSettingsAsync(CancellationToken cancellationToken = default);
    Task<AlarmDefinition> SaveAlarmAsync(AlarmDefinition alarm, CancellationToken cancellationToken = default);
    Task DeleteAlarmAsync(Guid alarmId, CancellationToken cancellationToken = default);
    Task SaveSettingsAsync(AppSettings settings, CancellationToken cancellationToken = default);
}

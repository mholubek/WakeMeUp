using Microsoft.Extensions.Logging.Abstractions;
using WakeMeUp.Domain;
using WakeMeUp.Services;

namespace WakeMeUp.Tests;

public sealed class BulkAlarmToggleServiceTests
{
    [Fact]
    public async Task SetAllAlarmsEnabledAsync_OnlyUpdatesChangedAlarms()
    {
        var store = new InMemoryAlarmStore(
            [
                new AlarmDefinition { Id = Guid.NewGuid(), Name = "A", IsEnabled = true, Time = new TimeOnly(6, 0) },
                new AlarmDefinition { Id = Guid.NewGuid(), Name = "B", IsEnabled = false, Time = new TimeOnly(7, 0) },
                new AlarmDefinition { Id = Guid.NewGuid(), Name = "C", IsEnabled = false, Time = new TimeOnly(8, 0) }
            ]);
        var service = new BulkAlarmToggleService(store, new AlarmMutationService(), NullLogger<BulkAlarmToggleService>.Instance);

        var result = await service.SetAllAlarmsEnabledAsync(true);

        Assert.Equal(2, result.UpdatedCount);
        Assert.All(await store.GetAlarmsAsync(), alarm => Assert.True(alarm.IsEnabled));
        Assert.Equal(1, store.SaveAlarmsCalls);
    }

    [Fact]
    public async Task SetAllAlarmsEnabledAsync_ReturnsZero_WhenNoAlarmNeedsChanging()
    {
        var store = new InMemoryAlarmStore(
            [
                new AlarmDefinition { Id = Guid.NewGuid(), Name = "A", IsEnabled = true, Time = new TimeOnly(6, 0) }
            ]);
        var service = new BulkAlarmToggleService(store, new AlarmMutationService(), NullLogger<BulkAlarmToggleService>.Instance);

        var result = await service.SetAllAlarmsEnabledAsync(true);

        Assert.Equal(0, result.UpdatedCount);
        Assert.Equal(0, store.SaveAlarmsCalls);
    }
}

using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using WakeMeUp.Domain;
using WakeMeUp.Services;

namespace WakeMeUp.Tests;

public sealed class InMemoryAlarmStore : IAlarmStore
{
    private readonly List<AlarmDefinition> _alarms = [];
    private AppSettings _settings = new();

    public int GetAlarmsCalls { get; private set; }
    public int SaveAlarmsCalls { get; private set; }

    public InMemoryAlarmStore(IEnumerable<AlarmDefinition>? alarms = null, AppSettings? settings = null)
    {
        Reset(alarms, settings);
    }

    public void Reset(IEnumerable<AlarmDefinition>? alarms = null, AppSettings? settings = null)
    {
        _alarms.Clear();
        if (alarms is not null)
        {
            _alarms.AddRange(alarms.Select(Clone));
        }

        _settings = settings is null
            ? new AppSettings()
            : new AppSettings
            {
                Language = settings.Language,
                ThemeMode = settings.ThemeMode,
                LanguageInitialized = settings.LanguageInitialized
            };

        GetAlarmsCalls = 0;
        SaveAlarmsCalls = 0;
    }

    public Task<AppState> GetStateAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(new AppState());

    public Task<IReadOnlyList<AlarmDefinition>> GetAlarmsAsync(CancellationToken cancellationToken = default)
    {
        GetAlarmsCalls++;
        return Task.FromResult<IReadOnlyList<AlarmDefinition>>(_alarms.Select(Clone).ToList());
    }

    public Task<AppSettings> GetSettingsAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(new AppSettings
        {
            Language = _settings.Language,
            ThemeMode = _settings.ThemeMode,
            LanguageInitialized = _settings.LanguageInitialized
        });

    public Task<AlarmDefinition> SaveAlarmAsync(AlarmDefinition alarm, CancellationToken cancellationToken = default)
    {
        var index = _alarms.FindIndex(existing => existing.Id == alarm.Id);
        if (index >= 0)
        {
            _alarms[index] = Clone(alarm);
        }
        else
        {
            _alarms.Add(Clone(alarm));
        }

        return Task.FromResult(Clone(alarm));
    }

    public Task SaveAlarmsAsync(IEnumerable<AlarmDefinition> alarms, CancellationToken cancellationToken = default)
    {
        SaveAlarmsCalls++;
        foreach (var alarm in alarms)
        {
            var index = _alarms.FindIndex(existing => existing.Id == alarm.Id);
            if (index >= 0)
            {
                _alarms[index] = Clone(alarm);
            }
            else
            {
                _alarms.Add(Clone(alarm));
            }
        }

        return Task.CompletedTask;
    }

    public Task DeleteAlarmAsync(Guid alarmId, CancellationToken cancellationToken = default)
    {
        _alarms.RemoveAll(alarm => alarm.Id == alarmId);
        return Task.CompletedTask;
    }

    public Task SaveSettingsAsync(AppSettings settings, CancellationToken cancellationToken = default)
    {
        _settings = new AppSettings
        {
            Language = settings.Language,
            ThemeMode = settings.ThemeMode,
            LanguageInitialized = settings.LanguageInitialized
        };
        return Task.CompletedTask;
    }

    private static AlarmDefinition Clone(AlarmDefinition alarm)
    {
        return new AlarmDefinition
        {
            Id = alarm.Id,
            Name = alarm.Name,
            Description = alarm.Description,
            Time = alarm.Time,
            IsEnabled = alarm.IsEnabled,
            RepeatMode = alarm.RepeatMode,
            Days = alarm.Days,
            CreatedUtc = alarm.CreatedUtc,
            LastProcessedOccurrenceUtc = alarm.LastProcessedOccurrenceUtc,
            LastTriggeredUtc = alarm.LastTriggeredUtc,
            LastResultMessage = alarm.LastResultMessage
        };
    }
}

internal sealed class TestHttpClientFactory : IHttpClientFactory
{
    private readonly Func<HttpRequestMessage, Task<HttpResponseMessage>> _handler;

    public TestHttpClientFactory(Func<HttpRequestMessage, Task<HttpResponseMessage>> handler)
    {
        _handler = handler;
    }

    public HttpClient CreateClient(string name)
    {
        return new HttpClient(new DelegateHttpMessageHandler(_handler), disposeHandler: true);
    }
}

internal sealed class DelegateHttpMessageHandler : HttpMessageHandler
{
    private readonly Func<HttpRequestMessage, Task<HttpResponseMessage>> _handler;

    public DelegateHttpMessageHandler(Func<HttpRequestMessage, Task<HttpResponseMessage>> handler)
    {
        _handler = handler;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return _handler(request);
    }
}

public sealed class WakeMeUpWebApplicationFactory : WebApplicationFactory<Program>
{
    public InMemoryAlarmStore Store { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IAlarmStore>();
            services.RemoveAll<IHostedService>();
            services.AddSingleton<IAlarmStore>(Store);
        });
    }
}

using System.Net.Http.Headers;
using System.Net.Http.Json;
using WakeMeUp.Domain;

namespace WakeMeUp.Services;

public sealed class HomeAssistantEventPublisher(
    IConfiguration configuration,
    IHttpClientFactory httpClientFactory,
    ILogger<HomeAssistantEventPublisher> logger)
{
    public const string AlarmTriggeredEventType = "wakemeup_alarm_triggered";
    private const string DefaultHomeAssistantBaseUrl = "http://supervisor/core/";

    public async Task<(bool Success, string Message)> PublishAlarmTriggeredAsync(
        AlarmDefinition alarm,
        DateTimeOffset scheduledOccurrence,
        CancellationToken cancellationToken)
    {
        var accessToken = configuration["SUPERVISOR_TOKEN"];
        var baseUrl = configuration["WakeMeUp:HomeAssistantBaseUrl"];

        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            baseUrl = DefaultHomeAssistantBaseUrl;
        }

        if (string.IsNullOrWhiteSpace(accessToken))
        {
            return (false, "SUPERVISOR_TOKEN is missing in the add-on environment.");
        }

        var payload = new
        {
            alarmId = alarm.Id,
            alarmName = alarm.Name,
            description = alarm.Description,
            repeatMode = alarm.RepeatMode.ToString(),
            scheduledLocal = scheduledOccurrence.ToString("O"),
            triggeredUtc = DateTimeOffset.UtcNow.ToString("O")
        };

        var client = httpClientFactory.CreateClient(nameof(HomeAssistantEventPublisher));
        client.BaseAddress = new Uri(AppendTrailingSlash(baseUrl));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.Trim());

        using var response = await client.PostAsJsonAsync($"api/events/{AlarmTriggeredEventType}", payload, cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            logger.LogInformation("Alarm {AlarmId} published Home Assistant event {EventType}", alarm.Id, AlarmTriggeredEventType);
            return (true, $"Event {AlarmTriggeredEventType} sent.");
        }

        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        logger.LogWarning("Home Assistant event publish failed for {AlarmId}. Status {StatusCode}. Body: {Body}", alarm.Id, response.StatusCode, body);
        return (false, $"Home Assistant returned {(int)response.StatusCode}.");
    }

    private static string AppendTrailingSlash(string value)
    {
        return value.EndsWith("/", StringComparison.Ordinal) ? value : $"{value}/";
    }
}

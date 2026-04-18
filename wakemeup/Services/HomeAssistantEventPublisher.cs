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
            name = alarm.Name,
            time = alarm.Time.ToString("HH\\:mm"),
            description = alarm.Description.Trim()
        };

        var client = httpClientFactory.CreateClient(nameof(HomeAssistantEventPublisher));
        client.BaseAddress = new Uri(AppendTrailingSlash(baseUrl));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.Trim());

        using var response = await client.PostAsJsonAsync($"api/events/{AlarmTriggeredEventType}", payload, cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            logger.LogInformation(
                "[{LoggedAt}] Alarm triggered: Name='{AlarmName}', Time='{AlarmTime}', Description='{AlarmDescription}'",
                GetLogTimestamp(),
                alarm.Name,
                alarm.Time.ToString("HH\\:mm"),
                GetLogDescription(alarm.Description));
            return (true, $"Event {AlarmTriggeredEventType} sent.");
        }

        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        logger.LogError(
            "[{LoggedAt}] Error triggering alarm: Name='{AlarmName}', Time='{AlarmTime}', Description='{AlarmDescription}', StatusCode={StatusCode}, Body='{ResponseBody}'",
            GetLogTimestamp(),
            alarm.Name,
            alarm.Time.ToString("HH\\:mm"),
            GetLogDescription(alarm.Description),
            (int)response.StatusCode,
            body);
        return (false, $"Home Assistant returned {(int)response.StatusCode}.");
    }

    private static string AppendTrailingSlash(string value)
    {
        return value.EndsWith("/", StringComparison.Ordinal) ? value : $"{value}/";
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

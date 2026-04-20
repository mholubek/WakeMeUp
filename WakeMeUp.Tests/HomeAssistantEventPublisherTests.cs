using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using WakeMeUp.Domain;
using WakeMeUp.Services;

namespace WakeMeUp.Tests;

public sealed class HomeAssistantEventPublisherTests
{
    [Fact]
    public async Task PublishAlarmTriggeredAsync_ReturnsFalse_WhenSupervisorTokenIsMissing()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["WakeMeUp:HomeAssistantBaseUrl"] = "http://homeassistant.local:8123/"
            })
            .Build();
        var publisher = new HomeAssistantEventPublisher(
            configuration,
            new TestHttpClientFactory(_ => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK))),
            NullLogger<HomeAssistantEventPublisher>.Instance);

        var result = await publisher.PublishAlarmTriggeredAsync(CreateAlarm(), DateTimeOffset.UtcNow, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal("SUPERVISOR_TOKEN is missing in the add-on environment.", result.Message);
    }

    [Fact]
    public async Task PublishAlarmTriggeredAsync_PostsExpectedPayload()
    {
        HttpRequestMessage? capturedRequest = null;
        object? capturedPayload = null;

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["SUPERVISOR_TOKEN"] = "token",
                ["WakeMeUp:HomeAssistantBaseUrl"] = "http://homeassistant.local:8123"
            })
            .Build();
        var publisher = new HomeAssistantEventPublisher(
            configuration,
            new TestHttpClientFactory(async request =>
            {
                capturedRequest = request;
                capturedPayload = await request.Content!.ReadFromJsonAsync<Dictionary<string, string>>();
                return new HttpResponseMessage(HttpStatusCode.OK);
            }),
            NullLogger<HomeAssistantEventPublisher>.Instance);

        var result = await publisher.PublishAlarmTriggeredAsync(CreateAlarm(), DateTimeOffset.UtcNow, CancellationToken.None);

        Assert.True(result.Success);
        Assert.Equal("Event wakemeup_alarm_triggered sent.", result.Message);
        Assert.NotNull(capturedRequest);
        Assert.Equal(HttpMethod.Post, capturedRequest!.Method);
        Assert.Equal("http://homeassistant.local:8123/api/events/wakemeup_alarm_triggered", capturedRequest.RequestUri!.ToString());
        Assert.Equal("Morning", ((Dictionary<string, string>)capturedPayload!)["name"]);
        Assert.Equal("07:00", ((Dictionary<string, string>)capturedPayload!)["time"]);
        Assert.Equal("Rise and shine", ((Dictionary<string, string>)capturedPayload!)["description"]);
    }

    private static AlarmDefinition CreateAlarm()
    {
        return new AlarmDefinition
        {
            Name = "Morning",
            Description = "  Rise and shine  ",
            Time = new TimeOnly(7, 0)
        };
    }
}

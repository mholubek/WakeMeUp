using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using WakeMeUp.Api;
using WakeMeUp.Domain;
using WakeMeUp.Services;

namespace WakeMeUp.Tests;

public sealed class AlarmApiIntegrationTests : IClassFixture<WakeMeUpWebApplicationFactory>
{
    private readonly WakeMeUpWebApplicationFactory _factory;

    public AlarmApiIntegrationTests(WakeMeUpWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetMeta_ReturnsConfiguredLanguageAndOptions()
    {
        _factory.Store.Reset(settings: new AppSettings { Language = AppLanguage.Slovak, ThemeMode = ThemeMode.Auto });
        using var client = _factory.CreateClient();

        var response = await client.GetFromJsonAsync<AlarmApiMetadataResponse>("/api/v1/meta");

        Assert.NotNull(response);
        Assert.Equal("slovak", response!.CurrentLanguage);
        Assert.Contains(response.RepeatModes, mode => mode.Value == "custom_days");
        Assert.Contains(response.Weekdays, day => day.Value == "monday");
    }

    [Fact]
    public async Task PostAlarm_CreatesAlarm_AndGetReturnsIt()
    {
        _factory.Store.Reset();
        using var client = _factory.CreateClient();

        var createRequest = new AlarmUpsertRequest("Created", "Test note", "07:30", true, "daily", []);
        var createResponse = await client.PostAsJsonAsync("/api/v1/alarms", createRequest);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = await createResponse.Content.ReadFromJsonAsync<AlarmResponse>();
        Assert.NotNull(created);
        Assert.Equal("Created", created!.Name);
        Assert.Equal("daily", created.RepeatMode);

        var fetched = await client.GetFromJsonAsync<AlarmResponse>($"/api/v1/alarms/{created.Id}");

        Assert.NotNull(fetched);
        Assert.Equal(created.Id, fetched!.Id);
        Assert.Equal("Created", fetched.Name);
    }

    [Fact]
    public async Task PostAlarm_ReturnsBadRequest_ForInvalidRepeatMode()
    {
        _factory.Store.Reset();
        using var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/v1/alarms",
            new AlarmUpsertRequest("Broken", string.Empty, "07:30", true, "broken", []));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
        Assert.NotNull(error);
        Assert.Equal("Validation failed.", error!.Error);
        Assert.Contains(error.Details, detail => detail.Field == "repeatMode");
    }

    [Fact]
    public async Task PutPatchAndDelete_WorkOnExistingAlarm()
    {
        var alarm = new AlarmDefinition
        {
            Id = Guid.NewGuid(),
            Name = "Initial",
            Description = "Before",
            Time = new TimeOnly(6, 45),
            IsEnabled = true,
            RepeatMode = RepeatMode.Never,
            CreatedUtc = DateTimeOffset.UtcNow
        };
        _factory.Store.Reset([alarm]);
        using var client = _factory.CreateClient();

        var updateResponse = await client.PutAsJsonAsync(
            $"/api/v1/alarms/{alarm.Id}",
            new AlarmUpsertRequest("Updated", "After", "08:15", false, "weekdays", []));
        var updated = await updateResponse.Content.ReadFromJsonAsync<AlarmResponse>();

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        Assert.NotNull(updated);
        Assert.Equal("Updated", updated!.Name);
        Assert.Equal("08:15", updated.Time);
        Assert.False(updated.IsEnabled);

        var toggleResponse = await client.PatchAsJsonAsync(
            $"/api/v1/alarms/{alarm.Id}/enabled",
            new AlarmToggleRequest(true));
        var toggled = await toggleResponse.Content.ReadFromJsonAsync<AlarmResponse>();

        Assert.Equal(HttpStatusCode.OK, toggleResponse.StatusCode);
        Assert.NotNull(toggled);
        Assert.True(toggled!.IsEnabled);

        var deleteResponse = await client.DeleteAsync($"/api/v1/alarms/{alarm.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        Assert.DoesNotContain(await _factory.Store.GetAlarmsAsync(), existing => existing.Id == alarm.Id);
    }
}

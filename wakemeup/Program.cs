using Microsoft.AspNetCore.DataProtection;
using WakeMeUp.Api;
using WakeMeUp.Components;
using WakeMeUp.Domain;
using WakeMeUp.Services;

var builder = WebApplication.CreateBuilder(args);
var isHomeAssistantAddon = Directory.Exists("/data");
var dataProtectionDirectory = isHomeAssistantAddon
    ? "/data/.aspnet/DataProtection-Keys"
    : Path.Combine(builder.Environment.ContentRootPath, "App_Data", "DataProtection-Keys");

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<UiTextService>();
builder.Services.AddScoped<AppState>();
builder.Services.AddScoped<BulkAlarmToggleService>();
builder.Services.AddSingleton<AlarmMutationService>();
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionDirectory))
    .SetApplicationName("WakeMeUp");
builder.Services.AddSingleton<IAlarmStore, SqliteAlarmStore>();
builder.Services.AddSingleton<AlarmOccurrenceService>();
builder.Services.AddSingleton<HomeAssistantEventPublisher>();
builder.Services.AddHostedService<AlarmScheduler>();

var app = builder.Build();

app.Use(async (context, next) =>
{
    if (context.Request.Headers.TryGetValue("X-Ingress-Path", out var ingressPath))
    {
        var pathBase = ingressPath.ToString().TrimEnd('/');

        if (!string.IsNullOrWhiteSpace(pathBase))
        {
            context.Request.PathBase = pathBase;
        }
    }

    await next();
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found");

if (!isHomeAssistantAddon)
{
    app.UseHttpsRedirection();
}

app.UseAntiforgery();

app.MapGet("/health", () => Results.Ok(new
{
    status = "ok",
    timestampUtc = DateTimeOffset.UtcNow
}));

app.MapAlarmApi();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

public partial class Program;

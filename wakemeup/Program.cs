using WakeMeUp.Components;
using WakeMeUp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
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
app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapGet("/health", () => Results.Ok(new
{
    status = "ok",
    timestampUtc = DateTimeOffset.UtcNow
}));

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

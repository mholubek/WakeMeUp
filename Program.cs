using WakeMeUp.Components;
using WakeMeUp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient();
builder.Services.AddSingleton<IAlarmStore, SqliteAlarmStore>();
builder.Services.AddSingleton<AlarmOccurrenceService>();
builder.Services.AddSingleton<HomeAssistantEventPublisher>();
builder.Services.AddHostedService<AlarmScheduler>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
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

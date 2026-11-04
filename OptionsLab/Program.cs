using Microsoft.Extensions.Options;
using OptionsLab.Diagnostics;
using OptionsLab.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IValidateOptions<NotificationOptions>, NotificationOptionsValidator>();
builder.Services.AddSingleton<IPostConfigureOptions<TenantBrandingOptions>, TenantBrandingPostConfigure>();
builder.Services.AddOptions<TenantBrandingOptions>()
    .Bind(builder.Configuration.GetRequiredSection("TenantBranding"))
    .ValidateDataAnnotations()
    .ValidateOnStart();
builder.Services.AddSingleton<SingletonBrandingDiagnostics>();
builder.Services.AddScoped<ScopedBrandingDiagnostics>();
builder.Services.AddSingleton<MonitorNotificationDiagnostics>();
builder.Services.AddHostedService(provider => provider.GetRequiredService<MonitorNotificationDiagnostics>());

ConfigureNotificationProfile(Options.DefaultName, "default");
ConfigureNotificationProfile("email", "email");
ConfigureNotificationProfile("sms", "sms");

var app = builder.Build();

app.MapGet("/", () => Results.Content(
    """
    <!doctype html>
    <html lang="en">
    <head><meta charset="utf-8"><title>Options Lab</title></head>
    <body>
      <main>
        <h1>Options Lab</h1>
        <p>Compare configuration lifetimes at <a href="/diagnostics"><code>/diagnostics</code></a>.</p>
        <section>
          <h2>IOptions&lt;T&gt;</h2>
          <p>Singleton view: use it when configuration should remain stable for the application's lifetime.</p>
        </section>
        <section>
          <h2>IOptionsSnapshot&lt;T&gt;</h2>
          <p>Scoped view: use it when each request needs a consistent configuration snapshot.</p>
        </section>
        <section>
          <h2>IOptionsMonitor&lt;T&gt;</h2>
          <p>Monitor view: use it in long-lived services that observe validated configuration changes.</p>
        </section>
        <p>Named notification profiles: <a href="/diagnostics/profiles/email">email</a> and <a href="/diagnostics/profiles/sms">sms</a>.</p>
      </main>
    </body>
    </html>
    """,
    "text/html"));

app.MapGet("/health", () => Results.Ok(new { status = "Healthy" }));
app.MapGet("/diagnostics", (
    SingletonBrandingDiagnostics singleton,
    ScopedBrandingDiagnostics scoped,
    MonitorNotificationDiagnostics monitor) =>
    Results.Ok(new
    {
        singleton = singleton.Describe(),
        scoped = scoped.Describe(),
        monitor = monitor.Describe()
    }));
app.MapGet("/diagnostics/profiles/{profileName}", (
    string profileName,
    MonitorNotificationDiagnostics monitor) =>
{
    if (!MonitorNotificationDiagnostics.ProfileNames.Contains(profileName, StringComparer.Ordinal))
    {
        return Results.BadRequest(new
        {
            error = $"Notification profile '{profileName}' is not configured."
        });
    }

    return monitor.TryGetProfile(profileName, out var profile)
        ? Results.Ok(profile)
        : Results.Problem(
            title: $"Notification profile '{profileName}' is not currently valid.",
            detail: monitor.ValidationError,
            statusCode: StatusCodes.Status503ServiceUnavailable);
});

app.Run();

void ConfigureNotificationProfile(string profileName, string configurationName)
{
    builder.Services.AddOptions<NotificationOptions>(profileName)
        .Bind(builder.Configuration.GetRequiredSection($"Notifications:{configurationName}"))
        .ValidateDataAnnotations()
        .ValidateOnStart();
}

public partial class Program;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using OptionsLab.Diagnostics;

namespace OptionsLab.Tests;

public sealed class OptionsLifetimeIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public OptionsLifetimeIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task DiagnosticsServices_ExposeTheirDocumentedScopes()
    {
        using var client = _factory.CreateClient();
        _ = await client.GetAsync("/health");

        var singleton = _factory.Services.GetRequiredService<SingletonBrandingDiagnostics>();
        var monitor = _factory.Services.GetRequiredService<MonitorNotificationDiagnostics>();
        using var firstScope = _factory.Services.CreateScope();
        using var secondScope = _factory.Services.CreateScope();
        var firstSnapshot = firstScope.ServiceProvider.GetRequiredService<ScopedBrandingDiagnostics>();
        var secondSnapshot = secondScope.ServiceProvider.GetRequiredService<ScopedBrandingDiagnostics>();

        Assert.Equal("IOptions<T>", singleton.Describe().Interface);
        Assert.Equal("application-singleton", singleton.Describe().ScopeId);
        Assert.Equal("IOptionsSnapshot<T>", firstSnapshot.Describe().Interface);
        Assert.NotEqual(firstSnapshot.ScopeId, secondSnapshot.ScopeId);
        Assert.Equal("IOptionsMonitor<T>", monitor.Describe().Interface);
        Assert.Contains(monitor.Describe().EffectiveProfiles, profile => profile.Profile == "email");
    }

    [Theory]
    [InlineData("email", "Email")]
    [InlineData("sms", "Sms")]
    public async Task NamedProfileDiagnostics_ReturnConfiguredProfile(string profileName, string provider)
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync($"/diagnostics/profiles/{profileName}");
        var content = await response.Content.ReadAsStringAsync();

        response.EnsureSuccessStatusCode();
        Assert.Contains($"\"provider\":\"{provider}\"", content, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task NamedProfileDiagnostics_RejectsUnknownProfile()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/diagnostics/profiles/unavailable");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("not configured", content);
    }

    [Fact]
    public async Task DashboardAndDiagnostics_IncludeAllThreeOptionsInterfaces()
    {
        using var client = _factory.CreateClient();

        var dashboard = await client.GetStringAsync("/");
        var diagnostics = await client.GetStringAsync("/diagnostics");

        Assert.Contains("IOptions&lt;T&gt;", dashboard);
        Assert.Contains("IOptionsSnapshot&lt;T&gt;", dashboard);
        Assert.Contains("IOptionsMonitor&lt;T&gt;", dashboard);
        Assert.Contains("IOptions<T>", diagnostics);
        Assert.Contains("IOptionsSnapshot<T>", diagnostics);
        Assert.Contains("IOptionsMonitor<T>", diagnostics);
    }
}

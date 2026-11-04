using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OptionsLab.Options;

namespace OptionsLab.Tests;

public sealed class OptionsBindingTests
{
    [Fact]
    public void NotificationOptions_BindsAndPassesAnnotationValidation()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Notifications:email:Provider"] = "Email",
                ["Notifications:email:SenderAddress"] = "alerts@example.test",
                ["Notifications:email:EmailFromAddress"] = "alerts@example.test"
            })
            .Build();
        var services = new ServiceCollection();

        services.AddOptions<NotificationOptions>("email")
            .Bind(configuration.GetSection("Notifications:email"))
            .ValidateDataAnnotations();

        using var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptionsMonitor<NotificationOptions>>();

        Assert.Equal("Email", options.Get("email").Provider);
        Assert.Equal("alerts@example.test", options.Get("email").SenderAddress);
    }

    [Fact]
    public void NotificationOptions_RejectsMissingSenderAddress()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Notifications:default:Provider"] = "Email",
                ["Notifications:default:SenderAddress"] = string.Empty,
                ["Notifications:default:EmailFromAddress"] = "alerts@example.test"
            })
            .Build();
        var services = new ServiceCollection();

        services.AddOptions<NotificationOptions>()
            .Bind(configuration.GetSection("Notifications:default"))
            .ValidateDataAnnotations();

        using var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<NotificationOptions>>();

        var exception = Assert.Throws<OptionsValidationException>(() => _ = options.Value);
        Assert.Contains("SenderAddress", exception.Message);
    }
}

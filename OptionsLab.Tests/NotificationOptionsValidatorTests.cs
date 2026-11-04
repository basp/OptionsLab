using Microsoft.Extensions.Options;
using OptionsLab.Options;

namespace OptionsLab.Tests;

public sealed class NotificationOptionsValidatorTests
{
    private readonly NotificationOptionsValidator _validator = new();

    [Fact]
    public void Validate_AcceptsEmailProfile()
    {
        var result = _validator.Validate("email", new NotificationOptions
        {
            Provider = "Email",
            SenderAddress = "alerts@example.test",
            EmailFromAddress = "alerts@example.test"
        });

        Assert.True(result.Succeeded);
    }

    [Fact]
    public void Validate_AcceptsSmsProfile()
    {
        var result = _validator.Validate("sms", new NotificationOptions
        {
            Provider = "Sms",
            SenderAddress = "OptionsLab",
            SmsSenderId = "OptionsLab"
        });

        Assert.True(result.Succeeded);
    }

    [Fact]
    public void Validate_RejectsProviderIncompatibleSettings()
    {
        var result = _validator.Validate("sms", new NotificationOptions
        {
            Provider = "Sms",
            SenderAddress = "OptionsLab",
            SmsSenderId = "OptionsLab",
            EmailFromAddress = "alerts@example.test"
        });

        Assert.False(result.Succeeded);
        Assert.Contains(result.Failures!, failure => failure.Contains("EmailFromAddress"));
    }
}

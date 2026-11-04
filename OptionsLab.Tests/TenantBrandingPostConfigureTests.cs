using OptionsLab.Options;

namespace OptionsLab.Tests;

public sealed class TenantBrandingPostConfigureTests
{
    [Fact]
    public void PostConfigure_DerivesSupportUrlWhenItIsMissing()
    {
        var options = new TenantBrandingOptions
        {
            TenantName = " Contoso Options Lab ",
            PrimaryColor = "#16a34a"
        };

        new TenantBrandingPostConfigure().PostConfigure(null, options);

        Assert.Equal("Contoso Options Lab", options.TenantName);
        Assert.Equal("#16A34A", options.PrimaryColor);
        Assert.Equal("https://support.example.test/contoso-options-lab", options.SupportUrl);
    }
}

using Microsoft.Extensions.Options;
using OptionsLab.Options;

namespace OptionsLab.Diagnostics;

public sealed class SingletonBrandingDiagnostics
{
    private readonly TenantBrandingOptions _branding;

    public SingletonBrandingDiagnostics(IOptions<TenantBrandingOptions> options)
    {
        _branding = options.Value;
    }

    public OptionsInterfaceDiagnostic Describe() =>
        new(
            "IOptions<T>",
            "Use in singleton services for values that stay stable for the application's lifetime.",
            "application-singleton",
            CreateBrandingView(_branding));

    internal static BrandingView CreateBrandingView(TenantBrandingOptions branding) =>
        new(branding.TenantName, branding.PrimaryColor, branding.SupportUrl);
}

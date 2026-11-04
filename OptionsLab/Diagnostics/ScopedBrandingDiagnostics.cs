using Microsoft.Extensions.Options;
using OptionsLab.Options;

namespace OptionsLab.Diagnostics;

public sealed class ScopedBrandingDiagnostics
{
    private readonly IOptionsSnapshot<TenantBrandingOptions> _options;

    public ScopedBrandingDiagnostics(IOptionsSnapshot<TenantBrandingOptions> options)
    {
        _options = options;
        ScopeId = Guid.NewGuid().ToString("N");
    }

    public string ScopeId { get; }

    public OptionsInterfaceDiagnostic Describe() =>
        new(
            "IOptionsSnapshot<T>",
            "Use in scoped services to read a configuration snapshot for the current request.",
            ScopeId,
            SingletonBrandingDiagnostics.CreateBrandingView(_options.Value));
}

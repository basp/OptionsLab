namespace OptionsLab.Diagnostics;

public sealed record BrandingView(string TenantName, string PrimaryColor, string? SupportUrl);

public sealed record OptionsInterfaceDiagnostic(
    string Interface,
    string RecommendedUse,
    string ScopeId,
    BrandingView Branding);

public sealed record NotificationProfileView(
    string Profile,
    string Provider,
    string SenderAddress,
    string? EmailFromAddress,
    string? SmsSenderId);

public sealed record MonitorDiagnostic(
    string Interface,
    string RecommendedUse,
    IReadOnlyCollection<NotificationProfileView> EffectiveProfiles,
    DateTimeOffset? LastEffectiveChangeUtc,
    string? ValidationError);

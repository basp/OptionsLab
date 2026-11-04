using System.Collections.Concurrent;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using OptionsLab.Options;

namespace OptionsLab.Diagnostics;

public sealed class MonitorNotificationDiagnostics : IHostedService, IDisposable
{
    public static readonly string[] ProfileNames = ["email", "sms"];

    private readonly IConfiguration _configuration;
    private readonly IOptionsMonitor<NotificationOptions> _options;
    private readonly ConcurrentDictionary<string, NotificationProfileView> _effectiveProfiles = new();
    private IDisposable? _optionsSubscription;
    private IDisposable? _reloadSubscription;

    public MonitorNotificationDiagnostics(
        IConfiguration configuration,
        IOptionsMonitor<NotificationOptions> options)
    {
        _configuration = configuration;
        _options = options;
    }

    public DateTimeOffset? LastEffectiveChangeUtc { get; private set; }

    public string? ValidationError { get; private set; }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        TrackCurrentProfiles();
        _optionsSubscription = _options.OnChange(RecordChangedProfile);
        _reloadSubscription = ChangeToken.OnChange(
            _configuration.GetReloadToken,
            TrackCurrentProfiles);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public MonitorDiagnostic Describe() =>
        new(
            "IOptionsMonitor<T>",
            "Use in long-lived services that must observe validated configuration changes.",
            _effectiveProfiles.Values.OrderBy(profile => profile.Profile).ToArray(),
            LastEffectiveChangeUtc,
            ValidationError);

    public bool TryGetProfile(string profileName, out NotificationProfileView? profile)
    {
        profile = _effectiveProfiles.GetValueOrDefault(profileName);
        return profile is not null;
    }

    public void Dispose()
    {
        _optionsSubscription?.Dispose();
        _reloadSubscription?.Dispose();
    }

    private void TrackCurrentProfiles()
    {
        string? validationError = null;

        foreach (var profileName in ProfileNames)
        {
            try
            {
                RecordEffectiveProfile(_options.Get(profileName), profileName, false);
            }
            catch (OptionsValidationException exception)
            {
                validationError = exception.Message;
            }
        }

        ValidationError = validationError;
    }

    private void RecordChangedProfile(NotificationOptions options, string? profileName)
    {
        RecordEffectiveProfile(options, profileName, true);
        TrackCurrentProfiles();
    }

    private void RecordEffectiveProfile(
        NotificationOptions options,
        string? profileName,
        bool isEffectiveChange)
    {
        var name = string.IsNullOrEmpty(profileName) ? "default" : profileName;
        _effectiveProfiles[name] = new NotificationProfileView(
            name,
            options.Provider,
            options.SenderAddress,
            options.EmailFromAddress,
            options.SmsSenderId);
        if (isEffectiveChange)
        {
            LastEffectiveChangeUtc = DateTimeOffset.UtcNow;
        }
    }
}

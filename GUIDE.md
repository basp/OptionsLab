# Options Lab Guide

Options Lab is a .NET 10 ASP.NET Core sample for observing configuration binding,
validation, named options, post-configuration, lifetimes, and JSON reloads. It
uses only safe example values and requires no credentials.

## Setup

From the repository root, restore, build, and run the sample:

```powershell
dotnet restore OptionsLab.slnx
dotnet build OptionsLab.slnx
$env:ASPNETCORE_ENVIRONMENT = "Development"
dotnet run --project OptionsLab\OptionsLab.csproj --no-launch-profile --urls http://127.0.0.1:5187
```

Open `http://127.0.0.1:5187/` for the dashboard. The following endpoints expose
the demonstrations:

| Endpoint | Observation |
| --- | --- |
| `/health` | Application health response. |
| `/diagnostics` | Singleton, scoped snapshot, and monitor views. |
| `/diagnostics/profiles/email` | The named email notification profile. |
| `/diagnostics/profiles/sms` | The named SMS notification profile. |

An unavailable profile returns `400 Bad Request` with an explanatory error.

## Configuration and binding

`OptionsLab\appsettings.json` supplies the baseline `Notifications` and
`TenantBranding` sections. `appsettings.Development.json` safely overrides the
development branding and email sender. The app binds:

| Options type | Configuration source | Registration | Expected observation |
| --- | --- | --- | --- |
| `NotificationOptions` | `Notifications:default`, `Notifications:email`, and `Notifications:sms` | Default and named `AddOptions` registrations | Email and SMS endpoints show separate configured profiles. |
| `TenantBrandingOptions` | `TenantBranding` | Standard `AddOptions` registration | Diagnostics show the effective tenant name, color, and support URL. |

`TenantBrandingOptions` uses post-configuration to trim its display name,
normalize its color, and derive `https://support.example.test/<tenant>` when
`SupportUrl` is omitted. The development sample intentionally omits
`SupportUrl` so this derived default is visible.

## Validation

Data annotations require a notification provider and sender, enforce the
provider value, validate email addresses, and constrain branding values.
`NotificationOptionsValidator` adds provider-specific rules:

- Email profiles require `EmailFromAddress` and cannot define `SmsSenderId`.
- SMS profiles require `SmsSenderId` and cannot define `EmailFromAddress`.

All configured notification profiles and branding use `ValidateOnStart`.
Missing or invalid initial settings prevent startup and identify the affected
option in the `OptionsValidationException`.

## Choosing an options interface

`/diagnostics` displays the same configuration through each appropriate
consumer lifetime:

| Interface | Demonstration | Recommended use | Expected observation |
| --- | --- | --- | --- |
| `IOptions<T>` | `SingletonBrandingDiagnostics` | Singleton services that need values stable for the app lifetime. | The `singleton` response has `application-singleton` as its scope identifier. |
| `IOptionsSnapshot<T>` | `ScopedBrandingDiagnostics` | Scoped/request services that need a consistent value for one request. | The `scoped` response has a new scope identifier on each request. |
| `IOptionsMonitor<T>` | `MonitorNotificationDiagnostics` | Long-lived services that observe validated changes. | The `monitor` response lists effective notification profiles, a last effective-change timestamp, and any reload validation error. |

## Reload walkthrough

Keep the development server running, then edit
`OptionsLab\appsettings.Development.json`. Change the email sender to another
safe address:

```json
"Notifications": {
  "email": {
    "SenderAddress": "alerts-reloaded@example.test"
  }
}
```

After the file watcher reloads the JSON, request `/diagnostics`. The
`monitor.effectiveProfiles` entry for `email` shows the new sender and
`monitor.lastEffectiveChangeUtc` advances.

To observe reload-time validation, temporarily add an invalid value:

```json
"EmailFromAddress": "not-an-email"
```

`/diagnostics` then reports `monitor.validationError`; the last valid email
profile remains in `monitor.effectiveProfiles` instead of exposing the invalid
setting. Restore the valid JSON when finished.

Some editors, filesystems, and containers delay or suppress file-watch events.
If a valid edit is not observed, stop the app with `Ctrl+C` and run the setup
command again. A restart reloads the current JSON configuration.

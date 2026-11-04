# OptionsLab

A .NET 10 ASP.NET Core sample for exploring the .NET options framework: configuration binding, validation, post-configuration, named options, options lifetimes, and live configuration reload. It uses only safe example values and needs no credentials.

## Features

- **Binding and defaults**: `NotificationOptions` (named `default`, `email`, `sms` profiles) and `TenantBrandingOptions` bound from JSON configuration.
- **Validation**: data annotations plus a provider-specific `IValidateOptions<NotificationOptions>`; all options use `ValidateOnStart`, so invalid configuration prevents startup.
- **Post-configuration**: `IPostConfigureOptions<TenantBrandingOptions>` normalizes values and derives a support URL.
- **Lifetimes**: `IOptions<T>` (singleton), `IOptionsSnapshot<T>` (per request), and `IOptionsMonitor<T>` (long-lived, reload-aware) demonstrations.
- **Reload**: edits to `appsettings.Development.json` are picked up while running; invalid edits surface a validation error without replacing the last valid values.

## Getting started

Requires the [.NET 10 SDK](https://dotnet.microsoft.com/download).

```powershell
dotnet build OptionsLab.slnx
$env:ASPNETCORE_ENVIRONMENT = "Development"
dotnet run --project OptionsLab\OptionsLab.csproj --no-launch-profile --urls http://127.0.0.1:5187
```

| Endpoint | Description |
| --- | --- |
| `/` | Dashboard |
| `/health` | Health check |
| `/diagnostics` | Singleton, snapshot, and monitor views |
| `/diagnostics/profiles/{name}` | Named notification profile (`email`, `sms`); unknown names return `400` |

## Tests

```powershell
dotnet test OptionsLab.slnx
```

## Documentation

See [GUIDE.md](GUIDE.md) for a walkthrough of each demonstration, including the reload exercise and its restart fallback.

## Project layout

- `OptionsLab/` – the web application (`Options/`, `Diagnostics/`, `Program.cs`)
- `OptionsLab.Tests/` – unit and integration tests
- `openspec/` – OpenSpec specs and archived change history

## License

See [LICENSE](LICENSE).

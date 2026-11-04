# Tasks

## 1. Scaffold the sample

- [x] 1.1 Create the .NET 10 ASP.NET Core Options Lab solution and project structure; verify `dotnet build` succeeds from the solution root.
- [x] 1.2 Add safe baseline and development JSON configuration for notification profiles and tenant branding; verify the application starts without environment secrets.
- [x] 1.3 Add the minimal dashboard shell and diagnostics route mapping; verify the root page and health endpoint return successfully.

## 2. Implement binding and validation

- [x] 2.1 Define notification and tenant branding options models with defaults and data-annotation constraints; verify focused binding and annotation-validation tests pass.
- [x] 2.2 Register default and named notification profiles from configuration with startup validation; verify invalid initial sender settings prevent application startup.
- [x] 2.3 Implement provider-specific `IValidateOptions<NotificationOptions>` rules; verify tests cover valid email and SMS profiles plus incompatible provider settings.
- [x] 2.4 Implement `IPostConfigureOptions<TenantBrandingOptions>` derived defaults; verify a missing optional branding value produces the documented effective result.

## 3. Demonstrate options lifetimes and named instances

- [x] 3.1 Add singleton, scoped, and monitor-backed diagnostic services using `IOptions<T>`, `IOptionsSnapshot<T>`, and `IOptionsMonitor<T>` respectively; verify integration tests show their documented scope behavior.
- [x] 3.2 Implement named-profile diagnostics with explicit unknown-profile handling; verify requests for `email`, `sms`, and an unavailable profile return the expected values or client error.
- [x] 3.3 Add dashboard views and JSON responses that identify each options interface's recommended use; verify the rendered page and endpoint payloads include all three demonstrations.

## 4. Demonstrate reload behavior

- [x] 4.1 Enable local JSON reload and implement monitor change tracking with the effective-change timestamp; verify a valid configuration update is reflected by monitor diagnostics.
- [x] 4.2 Surface reload-time `OptionsValidationException` results without displaying invalid settings as effective; verify an invalid configuration update produces the documented diagnostic error.
- [x] 4.3 Document the configuration-edit reload walkthrough and restart fallback in `GUIDE.md`; verify the documented steps work in a clean local checkout.

## 5. Finish learning materials and integration checks

- [x] 5.1 Create a root-level `GUIDE.md` covering setup, binding, validation, post-configuration, named options, each lifetime interface, reload, and expected observations; verify every dashboard demonstration has a corresponding guide entry.
- [x] 5.2 Run the focused unit and integration test projects plus `dotnet build`; verify all commands exit successfully.

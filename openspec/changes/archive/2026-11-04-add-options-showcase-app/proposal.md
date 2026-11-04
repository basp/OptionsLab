# Proposal

## Why

The .NET options framework is easiest to understand when its configuration binding, lifetimes, validation, named instances, and reload behavior can be observed in one realistic application. A small interactive sample will give developers concrete examples instead of isolated snippets.

## What Changes

- Add an ASP.NET Core "Options Lab" sample application that models configurable notification delivery and tenant branding.
- Demonstrate section binding, defaults, data-annotation and custom validation, startup validation, post-configuration, named options, and options lifetimes.
- Provide diagnostic endpoints and a browser dashboard that make the values exposed by `IOptions<T>`, `IOptionsSnapshot<T>`, and `IOptionsMonitor<T>` visible.
- Include a safe configuration-reload demonstration using JSON file changes and `IOptionsMonitor<T>.OnChange`.
- Add focused tests and documentation explaining which options interface is appropriate for each scenario.
- Add a root-level `GUIDE.md` that explains each Options Lab example and how to reproduce its behavior.

## Capabilities

### New Capabilities

- `options-lab`: Provides an observable ASP.NET Core application that demonstrates configuration-backed .NET options patterns and their runtime behavior.

### Modified Capabilities

- None.

## Impact

- Adds a new .NET sample project, configuration files, automated tests, and a root-level `GUIDE.md`.
- Uses the built-in Microsoft.Extensions.Options and configuration abstractions; no third-party runtime dependency is required.

# Design

## Context

This repository contains only OpenSpec scaffolding. The new sample will be a self-contained ASP.NET Core application. See `proposal.md` for motivation and `specs/options-lab/spec.md` for required behavior.

## Goals / Non-Goals

**Goals:**

- Make each major options feature executable and visible from a single local application.
- Keep configuration examples safe to run with no external service accounts or secrets.
- Demonstrate the lifetime semantics of `IOptions<T>`, `IOptionsSnapshot<T>`, and `IOptionsMonitor<T>` without misleading users about cache or reload behavior.
- Make invalid configuration fail early at startup and clearly at diagnostics time after a reload attempt.

**Non-Goals:**

- Delivering real email, SMS, or push notifications.
- Supporting remote configuration providers, multi-process synchronization, or production secrets management.
- Building a general-purpose options management UI.

## Decisions

### ASP.NET Core dashboard plus diagnostic API

The sample will use a .NET 10 LTS ASP.NET Core project with a small server-rendered dashboard and JSON diagnostics endpoints. The dashboard gives an approachable walkthrough, while endpoints make scope and reload behavior easy to test.

Alternatives considered:

- Console application: simpler but cannot naturally demonstrate request-scoped snapshots.
- Separate frontend: unnecessary complexity for an educational sample.

### Two configuration domains with named notification profiles

`NotificationOptions` will model provider-independent sender settings plus provider-specific values, with named `email` and `sms` instances. `TenantBrandingOptions` will model a normal configuration section with optional defaults. These domains let the sample distinguish basic binding, named options, and post-configuration without handling credentials.

Alternatives considered:

- A single options class: would blur the reason for named options and post-configuration.
- Database-backed tenant settings: distracts from configuration binding.

### Layered validation and startup enforcement

The app will bind from `appsettings.json` with reload-on-change enabled. Data annotations will enforce local shape constraints; an `IValidateOptions<NotificationOptions>` validator will enforce provider-specific cross-field rules. Every required profile will use `ValidateOnStart` so a bad initial configuration blocks startup. Diagnostics will surface `OptionsValidationException` after a reload rather than showing an invalid value.

Alternatives considered:

- Manual validation in endpoints: duplicates framework behavior and omits startup safety.
- Annotation-only validation: cannot express provider-dependent rules cleanly.

### Explicit options lifetime demonstrations

A singleton status service will use `IOptions<T>` to show an initial stable value. A scoped diagnostic service will use `IOptionsSnapshot<T>` and include a generated request/scope identifier. A hosted monitor service will use `IOptionsMonitor<T>` and `OnChange` to record valid observed changes and their timestamp. The dashboard will explain that snapshot values refresh at a new scope/request, while monitor values update on a configuration reload.

Alternatives considered:

- Injecting all three interfaces into one endpoint: technically compact but obscures their intended consumer lifetimes.

### Post-configuration for derived defaults

`IPostConfigureOptions<TenantBrandingOptions>` will normalize optional display data and derive a safe support URL when omitted. The resulting effective values will be displayed beside source configuration examples so users can see post-configuration's role.

Alternatives considered:

- Computing defaults in controllers: hides configuration composition outside the options pipeline.

### Root-level guided walkthrough

The sample will include `GUIDE.md` at its root as the durable companion to the dashboard. It will cover setup, the configuration-to-options pipeline, each live demonstration's interfaces and expected observations, validation failure behavior, named profiles, post-configuration, and the reload walkthrough. The guide will link to the relevant configuration and diagnostic endpoints, without duplicating implementation internals.

Alternatives considered:

- Dashboard-only guidance: does not give developers a durable, code-reviewable reference or a copyable local workflow.
- Separate documentation site: adds publishing overhead that is disproportionate for a self-contained sample.

### Reload workflow based on local JSON

`GUIDE.md` will instruct users to change a designated non-sensitive setting in `appsettings.Development.json` while the app runs. The monitor service will record valid change events. Invalid edits will be reported by the monitor diagnostics endpoint and will not be represented as successfully applied.

Alternatives considered:

- An in-app editor: would require file-writing permissions and add behavior unrelated to the framework lesson.

## Risks / Trade-offs

- [File watcher behavior varies across editors and containers] -> Document a restart fallback and provide a manual diagnostic refresh action.
- [Invalid reload semantics can be confusing] -> Clearly distinguish a validation error from an effective configuration update in the UI and API.
- [Options interfaces can be misused by consumers] -> Pair each live demonstration with a succinct recommended-use explanation and automated tests.
- [A dashboard can overemphasize visual polish] -> Keep the UI minimal; treat the diagnostics API and tests as the primary teaching artifacts.

## Migration Plan

1. Add the self-contained sample project, configuration, and documentation.
2. Verify startup validation, named profile selection, scoped diagnostics, and valid/invalid reload behavior with focused tests.
3. Publish as a new sample without modifying existing applications or shared configuration.

Rollback is removal of the new sample project and its documentation; no existing runtime behavior or persisted data is affected.

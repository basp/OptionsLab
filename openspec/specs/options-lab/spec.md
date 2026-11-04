# options-lab Specification

## Purpose

Provide a runnable, observable application for learning configuration-backed .NET options patterns safely.

## Requirements

### Requirement: Configurable sample domains
The application SHALL expose notification delivery settings and tenant branding settings from application configuration, with documented sample values and safe defaults for optional values.

#### Scenario: Settings are bound from configuration
- **WHEN** the application starts with valid notification and branding configuration
- **THEN** the dashboard and diagnostic API display the effective values for both settings domains

#### Scenario: Optional setting is absent
- **WHEN** an optional branding value is omitted from configuration
- **THEN** the application displays its documented default without failing startup

### Requirement: Configuration validity is enforced
The application SHALL prevent startup when required settings are absent or invalid, and it SHALL report validation failures with the affected setting and reason.

#### Scenario: Annotation validation fails
- **WHEN** a required notification sender address is missing or malformed
- **THEN** application startup fails with a message identifying the invalid notification setting

#### Scenario: Cross-field validation fails
- **WHEN** a notification provider selection conflicts with its provider-specific settings
- **THEN** application startup fails with a message explaining the incompatible combination

### Requirement: Options lifetimes are observable
The application SHALL provide a documented diagnostic flow that distinguishes stable singleton values, values refreshed per request scope, and values observed by a long-lived service.

#### Scenario: Diagnostics compare options views
- **WHEN** a user requests the options diagnostics endpoint
- **THEN** the response identifies the values and semantic lifetime represented by each options view

### Requirement: Named configuration instances are available
The application SHALL expose at least two independently configurable notification profiles and allow diagnostics to select each profile by name.

#### Scenario: A named profile is selected
- **WHEN** a user requests diagnostics for a configured notification profile name
- **THEN** the response contains the effective settings for that profile without returning another profile's values

#### Scenario: An unknown profile is selected
- **WHEN** a user requests diagnostics for a profile name that is not configured
- **THEN** the application returns a clear client error identifying the unavailable profile

### Requirement: Configuration reload is observable
The application SHALL detect valid changes to reloadable configuration and visibly report the current effective values and the most recent change time.

#### Scenario: Valid configuration is changed while running
- **WHEN** a reloadable notification setting is updated in the active configuration source
- **THEN** subsequent monitor diagnostics display the updated value and a newer observed-change time

#### Scenario: Invalid configuration is changed while running
- **WHEN** a reloadable configuration change produces invalid settings
- **THEN** the application retains or reports the validation failure without presenting the invalid values as effective settings

### Requirement: Learning guidance is included
The sample SHALL include a root-level `GUIDE.md` that maps every demonstrated options pattern to its configuration source, registration, validation behavior, lifetime or reload behavior, expected observation, and recommended use case.

#### Scenario: Developer follows setup documentation
- **WHEN** a developer follows `GUIDE.md`
- **THEN** they can run the application, inspect every demonstration, and trigger the reload demonstration without external credentials

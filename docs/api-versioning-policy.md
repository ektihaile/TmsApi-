# TMS API Versioning Policy

## 1. Breaking Changes

A change is breaking when an existing client can no longer use the API
without modifying its code. Examples include removing or renaming a JSON
field, changing an HTTP status code, tightening validation rules, or changing
the default sort order in a way that changes existing results.

Breaking changes require a new API version.

## 2. Additive Changes

Additive changes are non-breaking when existing clients continue to work
without modification. Examples include adding a new optional response field,
adding a new endpoint, or adding a new optional query parameter.

These changes may be released within the current API version.

## 3. Sunset Window

When V2 is released, V1 will remain available for a minimum of six months.
This gives clients, including rural training centres with quarterly
maintenance schedules, enough time to migrate and test their integrations.

The V1 shutdown date will be communicated before the sunset date.

## 4. Communication

From the day V2 is released, V1 responses will include:

- `Deprecation: true`
- `Sunset: <shutdown date>`
- `Link: <V2 URL>; rel="successor-version"`

Every version change will also be recorded in the CHANGELOG. Teams that hold
an API key will receive an email, and a calendar invite will be sent for the
V1 shutdown date.

## 5. Skipping Versions

Clients may migrate directly from V1 to V3 when V3 is the appropriate target.
Clients are not required to migrate through every intermediate API version.
Each version must have clear documentation and migration guidance.
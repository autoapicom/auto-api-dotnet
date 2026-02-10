# GitHub Copilot Instructions — auto-api-dotnet

This is a C# client library for the auto-api.com API.

## Architecture

- `AutoApiClient` is the main class, implements `IDisposable`
- 6 async methods for API endpoints (offers, changes, sources, etc.)
- Zero external dependencies — uses only `System.Net.Http` and `System.Text.Json`
- `CancellationToken` is always the last parameter with a default value
- Returns typed models: `OffersResponse`, `ChangesResponse`, `Meta`, etc.
- `JsonElement` is used for raw offer data that varies between sources

## Exceptions

- `ApiException` — base exception for all API errors
- `AuthException` — authentication/authorization errors (401/403)

## Code Style

- .NET 6+, nullable reference types enabled
- PascalCase for public members, `Async` suffix on async methods
- XML doc comments in English on every public type and method
- Never add NuGet dependencies or use Newtonsoft.Json

## Language

All code, comments, and documentation must be in English.

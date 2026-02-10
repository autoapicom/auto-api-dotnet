# Claude Instructions — auto-api-dotnet

## Language

All code comments, documentation, and README files must be written in **English**.

## Commands

- Build: `dotnet build`
- Run example: `dotnet run --project examples/Example`

## Key Files

- `src/AutoApi/AutoApiClient.cs` — main client class (6 async methods)
- `src/AutoApi/Models.cs` — response models (OffersResponse, ChangesResponse, Meta, etc.)
- `src/AutoApi/OffersParams.cs` — query parameters builder
- `src/AutoApi/Exceptions.cs` — ApiException, AuthException
- `src/AutoApi/AutoApi.csproj` — project file

## Code Style

- .NET 6+, C# 10+
- Zero dependencies — only `System.Net.Http` and `System.Text.Json`
- `AutoApiClient` implements `IDisposable`
- All API methods are `async` returning `Task<T>`
- `CancellationToken` as the last parameter with default value
- Nullable reference types enabled (`<Nullable>enable</Nullable>`)
- PascalCase for all public members
- `Async` suffix on all async methods
- XML doc comments in English on every public type and method
- `JsonElement` for raw offer data (varies between sources)

## Conventions

- Never add NuGet dependencies
- Never use Newtonsoft.Json
- Never skip CancellationToken parameter
- Keep it simple — no unnecessary abstractions

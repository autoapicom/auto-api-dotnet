# auto-api .NET Client

C# client for [auto-api.com](https://auto-api.com) — car listings API across 8 marketplaces.

## Quick Start

```bash
dotnet add package AutoApi.Client
```

```csharp
using AutoApi;

using var client = new AutoApiClient("your-api-key", "https://api1.auto-api.com");
var offers = await client.GetOffersAsync("encar", new OffersParams { Page = 1 });
```

## Build & Test

```bash
dotnet build
dotnet test
dotnet run --project examples/Example
```

## Key Files

- `src/AutoApi/AutoApiClient.cs` — AutoApiClient class, 6 async methods, HTTP helpers
- `src/AutoApi/OffersParams.cs` — Query parameters for offers endpoint
- `src/AutoApi/Models.cs` — OffersResponse, ChangesResponse, Meta, OfferItem, ChangeItem
- `src/AutoApi/Exceptions.cs` — ApiException and AuthException
- `src/AutoApi/AutoApi.csproj` — Project file (net6.0, NuGet metadata)
- `AutoApi.Client.sln` — Solution file

## Conventions

- .NET 6+, zero dependencies (System.Net.Http, System.Text.Json only)
- async/await with CancellationToken as last parameter of every public method
- Exceptions for errors: ApiException (base), AuthException (401/403)
- PascalCase for public members, Async suffix on async methods (C# convention)
- OfferItem.Data is JsonElement — structure varies between sources
- Nullable reference types enabled
- All comments are XML doc comments, written in English

## API Methods

| Method | Params | Returns |
|--------|--------|---------|
| `GetFiltersAsync(source)` | source name | `JsonElement` |
| `GetOffersAsync(source, params)` | source + OffersParams | `OffersResponse` |
| `GetOfferAsync(source, innerId)` | source + inner_id | `OffersResponse` |
| `GetChangeIdAsync(source, date)` | source + yyyy-mm-dd | `int` |
| `GetChangesAsync(source, changeId)` | source + change_id | `ChangesResponse` |
| `GetOfferByUrlAsync(url)` | marketplace URL | `JsonElement` |

# auto-api-client-dotnet

[![NuGet](https://img.shields.io/nuget/v/AutoApi.Client)](https://nuget.org/packages/AutoApi.Client)
[![.NET](https://img.shields.io/badge/.NET-6.0%2B-blue)](https://dotnet.microsoft.com)
[![License](https://img.shields.io/github/license/autoapicom/auto-api-dotnet)](LICENSE)

.NET client for the [auto-api.com](https://auto-api.com) car listings API — search offers, track changes and pull listing data from 8 automotive marketplaces worldwide.

Sources: encar (Korea), mobile.de, autoscout24 (Europe), che168, dongchedi, guazi (China), dubicars, dubizzle (UAE). Zero NuGet dependencies — built on `HttpClient` and `System.Text.Json`.

## Installation

```bash
dotnet add package AutoApi.Client
```

## Usage

```csharp
using AutoApi;

using var client = new AutoApiClient("your-api-key");
```

### Get filters

```csharp
var filters = await client.GetFiltersAsync("encar");
```

### Search offers

```csharp
var offers = await client.GetOffersAsync("mobilede", new OffersParams
{
    Page = 1,
    Brand = "BMW",
    YearFrom = 2020,
});

// Pagination
Console.WriteLine(offers.Meta.Page);
Console.WriteLine(offers.Meta.NextPage);
```

### Get single offer

```csharp
var offer = await client.GetOfferAsync("encar", "40427050");
```

### Track changes

```csharp
var changeId = await client.GetChangeIdAsync("encar", "2025-01-15");
var changes = await client.GetChangesAsync("encar", changeId);

// Next batch
var nextBatch = await client.GetChangesAsync("encar", changes.Meta.NextChangeId);
```

### Get offer by URL

```csharp
var info = await client.GetOfferByUrlAsync(
    "https://encar.com/dc/dc_cardetailview.do?carid=40427050");
```

### Decode offer data

The `Data` property is a `JsonElement` because different sources return different fields. Deserialize to your own type:

```csharp
foreach (var item in offers.Result)
{
    var data = item.Data.Deserialize<MyOfferData>();
    Console.WriteLine($"{data.Mark} {data.Model} {data.Year} — ${data.Price}");
}
```

### Error handling

```csharp
try
{
    var offers = await client.GetOffersAsync("encar", new OffersParams { Page = 1 });
}
catch (AuthException ex)
{
    // 401/403 — invalid API key
    Console.WriteLine($"{ex.StatusCode}: {ex.Message}");
}
catch (ApiException ex)
{
    // Any other API error
    Console.WriteLine($"{ex.StatusCode}: {ex.Message}");
    Console.WriteLine(ex.ResponseBody);
}
```

## Supported sources

| Source | Platform | Region |
|--------|----------|--------|
| `encar` | [encar.com](https://encar.com) | South Korea |
| `mobilede` | [mobile.de](https://mobile.de) | Germany |
| `autoscout24` | [autoscout24.com](https://autoscout24.com) | Europe |
| `che168` | [che168.com](https://che168.com) | China |
| `dongchedi` | [dongchedi.com](https://dongchedi.com) | China |
| `guazi` | [guazi.com](https://guazi.com) | China |
| `dubicars` | [dubicars.com](https://dubicars.com) | UAE |
| `dubizzle` | [dubizzle.com](https://dubizzle.com) | UAE |

## Other languages

| Language | Package |
|----------|---------|
| PHP | [auto-api/client](https://github.com/autoapicom/auto-api-php) |
| TypeScript | [@auto-api/client](https://github.com/autoapicom/auto-api-node) |
| Python | [auto-api-client](https://github.com/autoapicom/auto-api-python) |
| Go | [auto-api-go](https://github.com/autoapicom/auto-api-go) |
| Java | [auto-api-client](https://github.com/autoapicom/auto-api-java) |
| Ruby | [auto-api-client](https://github.com/autoapicom/auto-api-ruby) |
| Rust | [auto-api-client](https://github.com/autoapicom/auto-api-rust) |

## Documentation

[auto-api.com](https://auto-api.com)

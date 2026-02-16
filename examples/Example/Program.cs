// Auto API .NET Client — Complete usage example.
//
// Replace "your-api-key" with your actual API key from https://auto-api.com
//
// Run: dotnet run --project examples/Example

using System.Text.Json;
using AutoApi;

using var client = new AutoApiClient("your-api-key", "https://api1.auto-api.com");
var source = "encar";

// --- Get available filters ---

var filters = await client.GetFiltersAsync(source);
Console.WriteLine($"Filters: {filters}");

// --- Search offers with filters ---

var offers = await client.GetOffersAsync(source, new OffersParams
{
    Page = 1,
    Brand = "Hyundai",
    YearFrom = 2020,
    PriceTo = 50000,
});

Console.WriteLine($"\n--- Offers (page {offers.Meta.Page}) ---");
foreach (var item in offers.Result)
{
    Console.WriteLine($"[{item.ChangeType}] {item.InnerID} — {item.CreatedAt}");

    // Decode offer data into your own type if needed:
    // var data = item.Data.Deserialize<MyOfferData>();
}

// Pagination
if (offers.Meta.NextPage > 0)
{
    var nextPage = await client.GetOffersAsync(source, new OffersParams
    {
        Page = offers.Meta.NextPage,
        Brand = "Hyundai",
        YearFrom = 2020,
    });
    Console.WriteLine($"Next page has {nextPage.Result.Count} offers");
}

// --- Get single offer ---

var innerId = "40427050";
if (offers.Result.Count > 0)
{
    innerId = offers.Result[0].InnerID;
}

var offer = await client.GetOfferAsync(source, innerId);
Console.WriteLine($"\n--- Single offer ---");
if (offer.Result.Count > 0)
{
    var first = offer.Result[0];
    Console.WriteLine($"ID: {first.InnerID}, Type: {first.ChangeType}");
}

// --- Track changes ---

var changeId = await client.GetChangeIdAsync(source, "2025-01-15");
Console.WriteLine($"\n--- Changes from 2025-01-15 (change_id: {changeId}) ---");

var changes = await client.GetChangesAsync(source, changeId);
foreach (var change in changes.Result)
{
    Console.WriteLine($"[{change.ChangeType}] {change.InnerID}");
}

if (changes.Meta.NextChangeId > 0)
{
    var moreChanges = await client.GetChangesAsync(source, changes.Meta.NextChangeId);
    Console.WriteLine($"Next batch: {moreChanges.Result.Count} changes");
}

// --- Get offer by URL ---

var info = await client.GetOfferByUrlAsync(
    "https://www.encar.com/dc/dc_cardetailview.do?carid=40427050");
Console.WriteLine($"\n--- Offer by URL ---");
Console.WriteLine(info);

// --- Error handling ---

using var badClient = new AutoApiClient("invalid-key", "https://api1.auto-api.com");
try
{
    await badClient.GetOffersAsync("encar", new OffersParams { Page = 1 });
}
catch (AuthException ex)
{
    Console.WriteLine($"\nAuth error: {ex.Message} (HTTP {ex.StatusCode})");
}
catch (ApiException ex)
{
    Console.WriteLine($"\nAPI error: {ex.Message} (HTTP {ex.StatusCode})");
    Console.WriteLine($"Body: {ex.ResponseBody}");
}

using System.Net;
using System.Text.Json;
using Xunit;

namespace AutoApi.Tests;

public class AutoApiClientTests
{
    private static (AutoApiClient client, MockHandler handler) CreateClient(
        string body,
        HttpStatusCode status = HttpStatusCode.OK,
        string apiKey = "test-key",
        string apiVersion = "v2")
    {
        var handler = new MockHandler(body, status);
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api1.auto-api.com"),
        };
        var client = new AutoApiClient(apiKey, apiVersion: apiVersion, httpClient: httpClient);
        return (client, handler);
    }

    // ── GetFilters ──────────────────────────────────────────────

    [Fact]
    public async Task GetFiltersAsync_ReturnsParsedResponse()
    {
        var (client, _) = CreateClient("""{"brands":["Toyota","Honda"],"body_types":["sedan","suv"]}""");

        var result = await client.GetFiltersAsync("encar");

        Assert.Equal(JsonValueKind.Array, result.GetProperty("brands").ValueKind);
        Assert.Equal(2, result.GetProperty("brands").GetArrayLength());
    }

    [Fact]
    public async Task GetFiltersAsync_CallsCorrectEndpoint()
    {
        var (client, handler) = CreateClient("{}");

        await client.GetFiltersAsync("encar");

        Assert.Contains("/api/v2/encar/filters", handler.LastRequest!.RequestUri!.ToString());
    }

    [Fact]
    public async Task GetFiltersAsync_IncludesApiKey()
    {
        var (client, handler) = CreateClient("{}", apiKey: "my-secret-key");

        await client.GetFiltersAsync("encar");

        Assert.Contains("api_key=my-secret-key", handler.LastRequest!.RequestUri!.Query);
    }

    [Fact]
    public async Task GetFiltersAsync_UsesGetMethod()
    {
        var (client, handler) = CreateClient("{}");

        await client.GetFiltersAsync("encar");

        Assert.Equal(HttpMethod.Get, handler.LastRequest!.Method);
    }

    // ── GetOffers ───────────────────────────────────────────────

    [Fact]
    public async Task GetOffersAsync_ReturnsOffersData()
    {
        var json = """{"result":[{"id":1,"inner_id":"a1","change_type":"added","created_at":"2024-01-15","data":{}}],"meta":{"page":1,"next_page":2,"limit":20}}""";
        var (client, _) = CreateClient(json);

        var result = await client.GetOffersAsync("encar", new OffersParams { Page = 1 });

        Assert.Single(result.Result);
        Assert.Equal(1, result.Meta.Page);
    }

    [Fact]
    public async Task GetOffersAsync_PassesPageParameter()
    {
        var (client, handler) = CreateClient("""{"result":[],"meta":{"page":1,"next_page":0,"limit":20}}""");

        await client.GetOffersAsync("encar", new OffersParams { Page = 1 });

        Assert.Contains("page=1", handler.LastRequest!.RequestUri!.Query);
    }

    [Fact]
    public async Task GetOffersAsync_PassesFilterParameters()
    {
        var (client, handler) = CreateClient("""{"result":[],"meta":{"page":2,"next_page":0,"limit":20}}""");

        await client.GetOffersAsync("mobile_de", new OffersParams
        {
            Page = 2,
            Brand = "BMW",
            YearFrom = 2020,
            PriceTo = 50000,
        });

        var query = handler.LastRequest!.RequestUri!.Query;
        Assert.Contains("brand=BMW", query);
        Assert.Contains("year_from=2020", query);
        Assert.Contains("price_to=50000", query);
        Assert.Contains("page=2", query);
    }

    [Fact]
    public async Task GetOffersAsync_WorksWithNullParams()
    {
        var (client, _) = CreateClient("""{"result":[],"meta":{"page":0,"next_page":0,"limit":20}}""");

        var result = await client.GetOffersAsync("encar");

        Assert.NotNull(result);
    }

    // ── GetOffer ────────────────────────────────────────────────

    [Fact]
    public async Task GetOfferAsync_PassesInnerId()
    {
        var (client, handler) = CreateClient("""{"result":[{"id":1,"inner_id":"abc123","change_type":"","created_at":"","data":{}}],"meta":{"page":1,"next_page":0,"limit":20}}""");

        var result = await client.GetOfferAsync("encar", "abc123");

        Assert.Contains("inner_id=abc123", handler.LastRequest!.RequestUri!.Query);
        Assert.Single(result.Result);
    }

    // ── GetChangeId ─────────────────────────────────────────────

    [Fact]
    public async Task GetChangeIdAsync_ReturnsInteger()
    {
        var (client, _) = CreateClient("""{"change_id":42567}""");

        var result = await client.GetChangeIdAsync("encar", "2024-01-15");

        Assert.Equal(42567, result);
    }

    [Fact]
    public async Task GetChangeIdAsync_PassesDate()
    {
        var (client, handler) = CreateClient("""{"change_id":0}""");

        await client.GetChangeIdAsync("encar", "2024-01-15");

        Assert.Contains("date=2024-01-15", handler.LastRequest!.RequestUri!.Query);
    }

    [Fact]
    public async Task GetChangeIdAsync_ReturnsZero()
    {
        var (client, _) = CreateClient("""{"change_id":0}""");

        var result = await client.GetChangeIdAsync("encar", "2024-01-01");

        Assert.Equal(0, result);
    }

    // ── GetChanges ──────────────────────────────────────────────

    [Fact]
    public async Task GetChangesAsync_ReturnsChangesFeed()
    {
        var json = """{"result":[{"id":1,"inner_id":"new1","change_type":"added","created_at":"2024-01-15","data":{}}],"meta":{"cur_change_id":42567,"next_change_id":42568,"limit":500}}""";
        var (client, _) = CreateClient(json);

        var result = await client.GetChangesAsync("encar", 42567);

        Assert.Single(result.Result);
        Assert.Equal(42567, result.Meta.CurChangeId);
    }

    [Fact]
    public async Task GetChangesAsync_PassesChangeId()
    {
        var (client, handler) = CreateClient("""{"result":[],"meta":{"cur_change_id":0,"next_change_id":0,"limit":500}}""");

        await client.GetChangesAsync("encar", 42567);

        Assert.Contains("change_id=42567", handler.LastRequest!.RequestUri!.Query);
    }

    // ── GetOfferByUrl ───────────────────────────────────────────

    [Fact]
    public async Task GetOfferByUrlAsync_ReturnsOfferData()
    {
        var (client, _) = CreateClient("""{"brand":"BMW","model":"X5","price":45000}""");

        var result = await client.GetOfferByUrlAsync("https://www.encar.com/car/123");

        Assert.Equal("BMW", result.GetProperty("brand").GetString());
    }

    [Fact]
    public async Task GetOfferByUrlAsync_UsesPostMethod()
    {
        var (client, handler) = CreateClient("{}");

        await client.GetOfferByUrlAsync("https://example.com/car/123");

        Assert.Equal(HttpMethod.Post, handler.LastRequest!.Method);
    }

    [Fact]
    public async Task GetOfferByUrlAsync_UsesV1Endpoint()
    {
        var (client, handler) = CreateClient("{}");

        await client.GetOfferByUrlAsync("https://example.com/car/123");

        Assert.Contains("/api/v1/offer/info", handler.LastRequest!.RequestUri!.ToString());
    }

    [Fact]
    public async Task GetOfferByUrlAsync_SendsApiKeyInHeader()
    {
        var (client, handler) = CreateClient("{}", apiKey: "header-key");

        await client.GetOfferByUrlAsync("https://example.com/car/123");

        Assert.Equal("header-key", handler.LastRequest!.Headers.GetValues("x-api-key").First());
    }

    [Fact]
    public async Task GetOfferByUrlAsync_SendsUrlInBody()
    {
        var (client, handler) = CreateClient("{}");

        await client.GetOfferByUrlAsync("https://example.com/car/123");

        Assert.Contains("https://example.com/car/123", handler.LastRequestBody!);
    }

    [Fact]
    public async Task GetOfferByUrlAsync_DoesNotIncludeApiKeyInUrl()
    {
        var (client, handler) = CreateClient("{}");

        await client.GetOfferByUrlAsync("https://example.com/car/123");

        Assert.DoesNotContain("api_key", handler.LastRequest!.RequestUri!.ToString());
    }

    // ── Custom API version ──────────────────────────────────────

    [Fact]
    public async Task CustomApiVersion_UsesInEndpoint()
    {
        var (client, handler) = CreateClient("{}", apiVersion: "v3");

        await client.GetFiltersAsync("encar");

        Assert.Contains("/api/v3/encar/filters", handler.LastRequest!.RequestUri!.ToString());
    }

    // ── Error handling ──────────────────────────────────────────

    [Fact]
    public async Task ThrowsApiExceptionOnServerError()
    {
        var (client, _) = CreateClient(
            """{"message":"Internal server error"}""",
            HttpStatusCode.InternalServerError);

        await Assert.ThrowsAsync<ApiException>(() => client.GetFiltersAsync("encar"));
    }

    [Fact]
    public async Task ApiExceptionContainsStatusCode()
    {
        var (client, _) = CreateClient(
            """{"message":"Server error"}""",
            HttpStatusCode.InternalServerError);

        var ex = await Assert.ThrowsAsync<ApiException>(() => client.GetFiltersAsync("encar"));

        Assert.Equal(500, ex.StatusCode);
    }

    [Fact]
    public async Task ApiExceptionContainsResponseBody()
    {
        var body = """{"message":"Validation failed","errors":["invalid page"]}""";
        var (client, _) = CreateClient(body, HttpStatusCode.UnprocessableEntity);

        var ex = await Assert.ThrowsAsync<ApiException>(() => client.GetFiltersAsync("encar"));

        Assert.Contains("Validation failed", ex.ResponseBody);
    }

    [Fact]
    public async Task ApiExceptionUsesMessageFromBody()
    {
        var (client, _) = CreateClient(
            """{"message":"Custom error message"}""",
            HttpStatusCode.InternalServerError);

        var ex = await Assert.ThrowsAsync<ApiException>(() => client.GetFiltersAsync("encar"));

        Assert.Equal("Custom error message", ex.Message);
    }

    [Fact]
    public async Task ApiExceptionFallbackMessage()
    {
        var (client, _) = CreateClient(
            """{"error":"something"}""",
            HttpStatusCode.InternalServerError);

        var ex = await Assert.ThrowsAsync<ApiException>(() => client.GetFiltersAsync("encar"));

        Assert.Equal("API error: 500", ex.Message);
    }

    [Fact]
    public async Task ThrowsAuthExceptionOn401()
    {
        var (client, _) = CreateClient(
            """{"message":"Unauthorized"}""",
            HttpStatusCode.Unauthorized);

        await Assert.ThrowsAsync<AuthException>(() => client.GetFiltersAsync("encar"));
    }

    [Fact]
    public async Task ThrowsAuthExceptionOn403()
    {
        var (client, _) = CreateClient(
            """{"message":"Forbidden"}""",
            HttpStatusCode.Forbidden);

        await Assert.ThrowsAsync<AuthException>(() => client.GetOffersAsync("encar"));
    }

    [Fact]
    public async Task AuthExceptionIsAlsoApiException()
    {
        var (client, _) = CreateClient(
            """{"message":"Bad key"}""",
            HttpStatusCode.Unauthorized);

        var ex = await Assert.ThrowsAsync<AuthException>(() => client.GetFiltersAsync("encar"));

        Assert.IsAssignableFrom<ApiException>(ex);
    }

    [Fact]
    public async Task ThrowsApiExceptionOnInvalidJson()
    {
        var (client, _) = CreateClient("not json at all");

        var ex = await Assert.ThrowsAsync<ApiException>(() => client.GetFiltersAsync("encar"));

        Assert.Contains("Invalid JSON response", ex.Message);
    }

    [Fact]
    public async Task NotFoundReturnsApiExceptionNotAuth()
    {
        var (client, _) = CreateClient(
            """{"message":"Source not found"}""",
            HttpStatusCode.NotFound);

        var ex = await Assert.ThrowsAsync<ApiException>(() => client.GetFiltersAsync("unknown_source"));

        Assert.Equal(404, ex.StatusCode);
        Assert.IsNotType<AuthException>(ex);
    }

    // ── Dispose ─────────────────────────────────────────────────

    [Fact]
    public void Dispose_DoesNotThrowWithInjectedHttpClient()
    {
        var handler = new MockHandler("{}");
        var httpClient = new HttpClient(handler);
        var client = new AutoApiClient("test-key", httpClient: httpClient);

        client.Dispose();

        // Injected HttpClient should still be usable
        Assert.NotNull(httpClient);
    }
}

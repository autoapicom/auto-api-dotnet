using System.Net.Http.Json;
using System.Text.Json;
using System.Web;

namespace AutoApi;

/// <summary>
/// Client for the auto-api.com car listings API.
/// </summary>
public class AutoApiClient : IDisposable
{
    private readonly string _apiKey;
    private readonly string _baseUrl;
    private readonly string _apiVersion;
    private readonly HttpClient _httpClient;
    private readonly bool _ownsHttpClient;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = false,
    };

    /// <summary>
    /// Creates a new AutoApiClient with the given API key.
    /// </summary>
    /// <param name="apiKey">API key from auto-api.com.</param>
    /// <param name="baseUrl">Base URL (default: https://api1.auto-api.com).</param>
    /// <param name="apiVersion">API version (default: v2).</param>
    /// <param name="httpClient">Optional HttpClient instance. If provided, the client will not be disposed.</param>
    public AutoApiClient(
        string apiKey,
        string baseUrl = "https://api1.auto-api.com",
        string apiVersion = "v2",
        HttpClient? httpClient = null)
    {
        _apiKey = apiKey;
        _baseUrl = baseUrl.TrimEnd('/');
        _apiVersion = apiVersion;

        if (httpClient != null)
        {
            _httpClient = httpClient;
            _ownsHttpClient = false;
        }
        else
        {
            _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
            _ownsHttpClient = true;
        }
    }

    /// <summary>
    /// Returns available filters for a source (brands, models, body types, etc.)
    /// </summary>
    public async Task<JsonElement> GetFiltersAsync(
        string source,
        CancellationToken cancellationToken = default)
    {
        return await GetAsync<JsonElement>(
            $"api/{_apiVersion}/{source}/filters",
            null,
            cancellationToken);
    }

    /// <summary>
    /// Returns a paginated list of offers with optional filters.
    /// </summary>
    public async Task<OffersResponse> GetOffersAsync(
        string source,
        OffersParams? parameters = null,
        CancellationToken cancellationToken = default)
    {
        var query = EncodeParams(parameters);

        return await GetAsync<OffersResponse>(
            $"api/{_apiVersion}/{source}/offers",
            query,
            cancellationToken);
    }

    /// <summary>
    /// Returns a single offer by inner_id.
    /// </summary>
    public async Task<OffersResponse> GetOfferAsync(
        string source,
        string innerId,
        CancellationToken cancellationToken = default)
    {
        var query = new Dictionary<string, string> { ["inner_id"] = innerId };

        return await GetAsync<OffersResponse>(
            $"api/{_apiVersion}/{source}/offer",
            query,
            cancellationToken);
    }

    /// <summary>
    /// Returns a change_id for the given date (format: yyyy-mm-dd).
    /// </summary>
    public async Task<int> GetChangeIdAsync(
        string source,
        string date,
        CancellationToken cancellationToken = default)
    {
        var query = new Dictionary<string, string> { ["date"] = date };

        var result = await GetAsync<JsonElement>(
            $"api/{_apiVersion}/{source}/change_id",
            query,
            cancellationToken);

        return result.GetProperty("change_id").GetInt32();
    }

    /// <summary>
    /// Returns a changes feed (added/changed/removed) starting from change_id.
    /// </summary>
    public async Task<ChangesResponse> GetChangesAsync(
        string source,
        int changeId,
        CancellationToken cancellationToken = default)
    {
        var query = new Dictionary<string, string>
        {
            ["change_id"] = changeId.ToString(),
        };

        return await GetAsync<ChangesResponse>(
            $"api/{_apiVersion}/{source}/changes",
            query,
            cancellationToken);
    }

    /// <summary>
    /// Returns offer data by its URL on the marketplace.
    /// Uses POST /api/v1/offer/info with x-api-key header.
    /// </summary>
    public async Task<JsonElement> GetOfferByUrlAsync(
        string url,
        CancellationToken cancellationToken = default)
    {
        return await PostAsync<JsonElement>(
            "api/v1/offer/info",
            new { url },
            cancellationToken);
    }

    private async Task<T> GetAsync<T>(
        string endpoint,
        Dictionary<string, string>? query,
        CancellationToken cancellationToken)
    {
        query ??= new Dictionary<string, string>();
        query["api_key"] = _apiKey;

        var queryString = string.Join("&",
            query.Select(kv =>
                $"{HttpUtility.UrlEncode(kv.Key)}={HttpUtility.UrlEncode(kv.Value)}"));

        var requestUrl = $"{_baseUrl}/{endpoint}?{queryString}";

        var response = await _httpClient.GetAsync(requestUrl, cancellationToken);
        return await HandleResponseAsync<T>(response, cancellationToken);
    }

    private async Task<T> PostAsync<T>(
        string endpoint,
        object data,
        CancellationToken cancellationToken)
    {
        var requestUrl = $"{_baseUrl}/{endpoint}";
        var content = JsonContent.Create(data);

        var request = new HttpRequestMessage(HttpMethod.Post, requestUrl)
        {
            Content = content,
        };
        request.Headers.Add("x-api-key", _apiKey);

        var response = await _httpClient.SendAsync(request, cancellationToken);
        return await HandleResponseAsync<T>(response, cancellationToken);
    }

    private async Task<T> HandleResponseAsync<T>(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var message = $"API error: {(int)response.StatusCode}";

            try
            {
                using var doc = JsonDocument.Parse(body);
                if (doc.RootElement.TryGetProperty("message", out var msgElement))
                {
                    message = msgElement.GetString() ?? message;
                }
            }
            catch (JsonException)
            {
                // Use default message if response is not valid JSON
            }

            var statusCode = (int)response.StatusCode;

            if (statusCode == 401 || statusCode == 403)
            {
                throw new AuthException(statusCode, message, body);
            }

            throw new ApiException(statusCode, message, body);
        }

        try
        {
            var result = JsonSerializer.Deserialize<T>(body, JsonOptions);
            return result!;
        }
        catch (JsonException)
        {
            throw new ApiException(
                (int)response.StatusCode,
                $"Invalid JSON response: {body[..Math.Min(body.Length, 200)]}",
                body);
        }
    }

    private static Dictionary<string, string> EncodeParams(OffersParams? parameters)
    {
        var query = new Dictionary<string, string>();

        if (parameters == null)
            return query;

        query["page"] = parameters.Page.ToString();

        if (parameters.Brand != null) query["brand"] = parameters.Brand;
        if (parameters.Model != null) query["model"] = parameters.Model;
        if (parameters.Configuration != null) query["configuration"] = parameters.Configuration;
        if (parameters.Complectation != null) query["complectation"] = parameters.Complectation;
        if (parameters.Transmission != null) query["transmission"] = parameters.Transmission;
        if (parameters.Color != null) query["color"] = parameters.Color;
        if (parameters.BodyType != null) query["body_type"] = parameters.BodyType;
        if (parameters.EngineType != null) query["engine_type"] = parameters.EngineType;
        if (parameters.YearFrom != null) query["year_from"] = parameters.YearFrom.Value.ToString();
        if (parameters.YearTo != null) query["year_to"] = parameters.YearTo.Value.ToString();
        if (parameters.MileageFrom != null) query["mileage_from"] = parameters.MileageFrom.Value.ToString();
        if (parameters.MileageTo != null) query["mileage_to"] = parameters.MileageTo.Value.ToString();
        if (parameters.PriceFrom != null) query["price_from"] = parameters.PriceFrom.Value.ToString();
        if (parameters.PriceTo != null) query["price_to"] = parameters.PriceTo.Value.ToString();

        return query;
    }

    /// <summary>
    /// Disposes the internal HttpClient if it was created by this instance.
    /// </summary>
    public void Dispose()
    {
        if (_ownsHttpClient)
        {
            _httpClient.Dispose();
        }

        GC.SuppressFinalize(this);
    }
}

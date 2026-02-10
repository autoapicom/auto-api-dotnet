using System.Text.Json;
using System.Text.Json.Serialization;

namespace AutoApi;

/// <summary>
/// Response from GetOffersAsync and GetOfferAsync.
/// </summary>
public class OffersResponse
{
    [JsonPropertyName("result")]
    public List<OfferItem> Result { get; set; } = new();

    [JsonPropertyName("meta")]
    public Meta Meta { get; set; } = new();
}

/// <summary>
/// Response from GetChangesAsync.
/// </summary>
public class ChangesResponse
{
    [JsonPropertyName("result")]
    public List<ChangeItem> Result { get; set; } = new();

    [JsonPropertyName("meta")]
    public ChangesMeta Meta { get; set; } = new();
}

/// <summary>
/// Pagination metadata for offers.
/// </summary>
public class Meta
{
    [JsonPropertyName("page")]
    public int Page { get; set; }

    [JsonPropertyName("next_page")]
    public int NextPage { get; set; }

    [JsonPropertyName("limit")]
    public int Limit { get; set; }
}

/// <summary>
/// Pagination metadata for changes feed.
/// </summary>
public class ChangesMeta
{
    [JsonPropertyName("cur_change_id")]
    public int CurChangeId { get; set; }

    [JsonPropertyName("next_change_id")]
    public int NextChangeId { get; set; }

    [JsonPropertyName("limit")]
    public int Limit { get; set; }
}

/// <summary>
/// A single item in the offers result array.
/// Data is a JsonElement because the structure varies between sources.
/// </summary>
public class OfferItem
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("inner_id")]
    public string InnerID { get; set; } = "";

    [JsonPropertyName("change_type")]
    public string ChangeType { get; set; } = "";

    [JsonPropertyName("created_at")]
    public string CreatedAt { get; set; } = "";

    [JsonPropertyName("data")]
    public JsonElement Data { get; set; }
}

/// <summary>
/// A single item in the changes result array.
/// Data is a JsonElement because the structure varies between sources.
/// </summary>
public class ChangeItem
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("inner_id")]
    public string InnerID { get; set; } = "";

    [JsonPropertyName("change_type")]
    public string ChangeType { get; set; } = "";

    [JsonPropertyName("created_at")]
    public string CreatedAt { get; set; } = "";

    [JsonPropertyName("data")]
    public JsonElement Data { get; set; }
}

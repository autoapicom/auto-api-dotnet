namespace AutoApi;

/// <summary>
/// Query parameters for GetOffersAsync.
/// Only non-null values are sent to the API.
/// </summary>
public class OffersParams
{
    /// <summary>
    /// Page number (required).
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// Filter by brand name.
    /// </summary>
    public string? Brand { get; set; }

    /// <summary>
    /// Filter by model name.
    /// </summary>
    public string? Model { get; set; }

    /// <summary>
    /// Filter by configuration.
    /// </summary>
    public string? Configuration { get; set; }

    /// <summary>
    /// Filter by complectation.
    /// </summary>
    public string? Complectation { get; set; }

    /// <summary>
    /// Filter by transmission type.
    /// </summary>
    public string? Transmission { get; set; }

    /// <summary>
    /// Filter by color.
    /// </summary>
    public string? Color { get; set; }

    /// <summary>
    /// Filter by body type.
    /// </summary>
    public string? BodyType { get; set; }

    /// <summary>
    /// Filter by engine type.
    /// </summary>
    public string? EngineType { get; set; }

    /// <summary>
    /// Minimum year filter.
    /// </summary>
    public int? YearFrom { get; set; }

    /// <summary>
    /// Maximum year filter.
    /// </summary>
    public int? YearTo { get; set; }

    /// <summary>
    /// Minimum mileage filter.
    /// </summary>
    public int? MileageFrom { get; set; }

    /// <summary>
    /// Maximum mileage filter.
    /// </summary>
    public int? MileageTo { get; set; }

    /// <summary>
    /// Minimum price filter.
    /// </summary>
    public int? PriceFrom { get; set; }

    /// <summary>
    /// Maximum price filter.
    /// </summary>
    public int? PriceTo { get; set; }
}

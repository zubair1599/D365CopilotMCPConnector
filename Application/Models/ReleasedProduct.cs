using System.Text.Json.Serialization;

namespace McpD365Server.Application.Models;

/// <summary>
/// Released product from ReleasedProductsV2 — exact D365 OData schema.
/// </summary>
public sealed class ReleasedProduct
{
    [JsonPropertyName("ProductNumber")]
    public string ProductNumber { get; set; } = string.Empty;

    [JsonPropertyName("SearchName")]
    public string SearchName { get; set; } = string.Empty;

    [JsonPropertyName("ProductType")]
    public string ProductType { get; set; } = string.Empty;

    [JsonPropertyName("SalesPrice")]
    public decimal SalesPrice { get; set; }

    [JsonPropertyName("ProductLifecycleStateId")]
    public string ProductLifecycleStateId { get; set; } = string.Empty;
}

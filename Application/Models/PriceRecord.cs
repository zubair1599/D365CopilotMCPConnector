using System.Text.Json.Serialization;

namespace McpD365Server.Application.Models;

/// <summary>
/// Price/discount record from PriceDiscAdmTrans — 5 key properties only.
/// </summary>
public sealed class PriceRecord
{
    [JsonPropertyName("ItemRelation")]
    public string ItemRelation { get; set; } = string.Empty;

    [JsonPropertyName("Amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("Currency")]
    public string Currency { get; set; } = string.Empty;

    [JsonPropertyName("FromDate")]
    public string FromDate { get; set; } = string.Empty;

    [JsonPropertyName("ToDate")]
    public string ToDate { get; set; } = string.Empty;
}

using System.Text.Json.Serialization;

namespace McpD365Server.Application.Models;

/// <summary>
/// Inventory on-hand from WarehousesOnHandV2 — exact D365 OData entity.
/// Uses generic property names to avoid schema mismatches.
/// </summary>
public sealed class InventoryOnHand
{
    [JsonPropertyName("ItemNumber")]
    public string ItemNumber { get; set; } = string.Empty;

    [JsonPropertyName("ProductName")]
    public string ProductName { get; set; } = string.Empty;

    [JsonPropertyName("PhysicalInventory")]
    public decimal PhysicalInventory { get; set; }

    [JsonPropertyName("PhysicalAvailable")]
    public decimal PhysicalAvailable { get; set; }

    [JsonPropertyName("WarehouseId")]
    public string WarehouseId { get; set; } = string.Empty;
}

using System.ComponentModel;
using System.Text.Json;
using McpD365Server.Application.Interfaces;
using McpD365Server.Application.Models;
using ModelContextProtocol.Server;

namespace McpD365Server.Application.Tools;

/// <summary>
/// MCP Tool: Check inventory on-hand from WarehousesOnHandV2.
/// Does NOT use $select — lets D365 return all fields to avoid property name mismatches.
/// </summary>
[McpServerToolType]
public sealed class CheckInventoryTool
{
    [McpServerTool(Name = "CheckInventory"), Description(
        "Check inventory on-hand quantities for one or more products in Dynamics 365. " +
        "Provide a single item number or multiple comma-separated item numbers. " +
        "Returns item number, product name, available quantity, on-hand quantity, and warehouse.")]
    public static async Task<string> CheckInventoryAsync(
        [Description("One or more item numbers. Comma-separated for batch (e.g. 'D0001' or 'D0001,D0002').")] string itemNumbers,
        ID365Client d365Client,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(itemNumbers))
            return JsonSerializer.Serialize(new { error = "The item number is required." });

        var ids = itemNumbers.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (var id in ids)
        {
            if (id.Contains('\'') || id.Contains(';'))
                return JsonSerializer.Serialize(new { error = "Invalid characters in item number." });
        }

        string filter;
        if (ids.Length == 1)
            filter = $"$filter=ItemNumber eq '{ids[0]}'";
        else
        {
            var conditions = ids.Select(id => $"ItemNumber eq '{id}'");
            filter = $"$filter={string.Join(" or ", conditions)}";
        }

        // No $select — let D365 return all fields to avoid property name mismatches
        var path = $"data/WarehousesOnHandV2?{filter}&$top=10";
        var result = await d365Client.GetAsync<InventoryOnHand>(path, cancellationToken);

        if (result.IsFailure)
            return JsonSerializer.Serialize(new { error = result.Error.Message });

        return JsonSerializer.Serialize(new
        {
            count = result.Value.Value.Length,
            inventory = result.Value.Value
        });
    }
}

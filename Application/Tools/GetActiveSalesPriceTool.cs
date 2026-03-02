using System.ComponentModel;
using System.Text.Json;
using McpD365Server.Application.Interfaces;
using McpD365Server.Application.Models;
using ModelContextProtocol.Server;

namespace McpD365Server.Application.Tools;

/// <summary>
/// MCP Tool: Get active sales prices only from PriceDiscAdmTrans.
/// </summary>
[McpServerToolType]
public sealed class GetActiveSalesPriceTool
{
    [McpServerTool(Name = "GetActiveSalesPrice"), Description(
        "Get currently active sales prices for a product from Dynamics 365. " +
        "Returns only pricing valid as of today, including amount, currency, and dates.")]
    public static async Task<string> GetActiveSalesPriceAsync(
        [Description("The item relation (product number) to look up active sales prices for.")] string itemRelation,
        ID365Client d365Client,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(itemRelation))
            return JsonSerializer.Serialize(new { error = "The item relation is required." });

        if (itemRelation.Contains('\'') || itemRelation.Contains(';'))
            return JsonSerializer.Serialize(new { error = "Invalid characters in item relation." });

        var today = DateTime.UtcNow.ToString("yyyy-MM-dd");

        var path = $"data/PriceDiscAdmTrans?$filter=ItemRelation eq '{itemRelation}' " +
                   $"and FromDate le {today} and (ToDate ge {today} or ToDate eq null)" +
                   "&$select=ItemRelation,Amount,Currency,FromDate,ToDate&$top=10";

        var result = await d365Client.GetAsync<PriceRecord>(path, cancellationToken);

        if (result.IsFailure)
            return JsonSerializer.Serialize(new { error = result.Error.Message });

        return JsonSerializer.Serialize(new
        {
            asOfDate = today,
            count = result.Value.Value.Length,
            activePrices = result.Value.Value
        });
    }
}

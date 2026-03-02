using System.ComponentModel;
using System.Text.Json;
using McpD365Server.Application.Interfaces;
using McpD365Server.Application.Models;
using ModelContextProtocol.Server;

namespace McpD365Server.Application.Tools;

/// <summary>
/// MCP Tool: Get product price from PriceDiscAdmTrans.
/// </summary>
[McpServerToolType]
public sealed class GetProductPriceTool
{
    [McpServerTool(Name = "GetProductPrice"), Description(
        "Get price information for a product from Dynamics 365. " +
        "Returns price records including amount, currency, and validity dates.")]
    public static async Task<string> GetProductPriceAsync(
        [Description("The item relation (product number) to look up prices for.")] string itemRelation,
        ID365Client d365Client,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(itemRelation))
            return JsonSerializer.Serialize(new { error = "The item relation is required." });

        if (itemRelation.Contains('\'') || itemRelation.Contains(';'))
            return JsonSerializer.Serialize(new { error = "Invalid characters in item relation." });

        var path = $"data/PriceDiscAdmTrans?$filter=ItemRelation eq '{itemRelation}'" +
                   "&$select=ItemRelation,Amount,Currency,FromDate,ToDate&$top=10";
        var result = await d365Client.GetAsync<PriceRecord>(path, cancellationToken);

        if (result.IsFailure)
            return JsonSerializer.Serialize(new { error = result.Error.Message });

        return JsonSerializer.Serialize(new
        {
            count = result.Value.Value.Length,
            prices = result.Value.Value
        });
    }
}

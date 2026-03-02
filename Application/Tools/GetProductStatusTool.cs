using System.ComponentModel;
using System.Text.Json;
using McpD365Server.Application.Interfaces;
using McpD365Server.Application.Models;
using ModelContextProtocol.Server;

namespace McpD365Server.Application.Tools;

/// <summary>
/// MCP Tool: Get product lifecycle state.
/// </summary>
[McpServerToolType]
public sealed class GetProductStatusTool
{
    [McpServerTool(Name = "GetProductStatus"), Description(
        "Get the lifecycle state of a released product in Dynamics 365. " +
        "Returns the product number, name, and its lifecycle state.")]
    public static async Task<string> GetProductStatusAsync(
        [Description("The exact product number to check status for.")] string productNumber,
        ID365Client d365Client,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(productNumber))
            return JsonSerializer.Serialize(new { error = "The product number is required." });

        if (productNumber.Contains('\'') || productNumber.Contains(';'))
            return JsonSerializer.Serialize(new { error = "Invalid characters in product number." });

        var path = $"data/ReleasedProductsV2?$filter=ProductNumber eq '{productNumber}'" +
                   "&$select=ProductNumber,SearchName,ProductLifecycleStateId&$top=1";
        var result = await d365Client.GetAsync<ReleasedProduct>(path, cancellationToken);

        if (result.IsFailure)
            return JsonSerializer.Serialize(new { error = result.Error.Message });

        if (result.Value.Value.Length == 0)
            return JsonSerializer.Serialize(new { error = $"No product found with number '{productNumber}'." });

        var product = result.Value.Value[0];

        return JsonSerializer.Serialize(new
        {
            productNumber = product.ProductNumber,
            productName = product.SearchName,
            lifecycleState = string.IsNullOrWhiteSpace(product.ProductLifecycleStateId)
                ? "Not Set"
                : product.ProductLifecycleStateId
        });
    }
}

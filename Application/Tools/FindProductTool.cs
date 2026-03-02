using System.ComponentModel;
using System.Text.Json;
using McpD365Server.Application.Interfaces;
using McpD365Server.Application.Models;
using ModelContextProtocol.Server;

namespace McpD365Server.Application.Tools;

/// <summary>
/// MCP Tool: Find a product by exact product number.
/// </summary>
[McpServerToolType]
public sealed class FindProductTool
{
    [McpServerTool(Name = "FindProduct"), Description(
        "Find a released product in Dynamics 365 by its exact product number. " +
        "Returns product number, name, type, sales price, and lifecycle state.")]
    public static async Task<string> FindProductAsync(
        [Description("The exact product number to search for.")] string productNumber,
        ID365Client d365Client,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(productNumber))
            return JsonSerializer.Serialize(new { error = "The product number is required." });

        if (productNumber.Contains('\'') || productNumber.Contains(';'))
            return JsonSerializer.Serialize(new { error = "Invalid characters in product number." });

        var path = $"data/ReleasedProductsV2?$filter=ProductNumber eq '{productNumber}'" +
                   "&$select=ProductNumber,SearchName,ProductType,SalesPrice,ProductLifecycleStateId&$top=10";
        var result = await d365Client.GetAsync<ReleasedProduct>(path, cancellationToken);

        if (result.IsFailure)
            return JsonSerializer.Serialize(new { error = result.Error.Message });

        if (result.Value.Value.Length == 0)
            return JsonSerializer.Serialize(new { error = $"No product found with number '{productNumber}'." });

        return JsonSerializer.Serialize(new
        {
            count = result.Value.Value.Length,
            products = result.Value.Value.Select(p => new
            {
                productNumber = p.ProductNumber,
                productName = p.SearchName,
                productType = p.ProductType,
                salesPrice = p.SalesPrice,
                lifecycleState = p.ProductLifecycleStateId
            })
        });
    }
}

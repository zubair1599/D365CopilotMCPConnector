using System.ComponentModel;
using System.Text.Json;
using McpD365Server.Application.Interfaces;
using McpD365Server.Application.Models;
using ModelContextProtocol.Server;

namespace McpD365Server.Application.Tools;

/// <summary>
/// MCP Tool: Count total released products in D365.
/// No parameters — returns total count of all products.
/// </summary>
[McpServerToolType]
public sealed class CountProductsTool
{
    [McpServerTool(Name = "CountProducts"), Description(
        "Count the total number of released products in Dynamics 365. Returns the total product count.")]
    public static async Task<string> CountProductsAsync(
        ID365Client d365Client,
        CancellationToken cancellationToken)
    {
        var path = "data/ReleasedProductsV2?$count=true&$top=0";
        var result = await d365Client.GetWithCountAsync<ReleasedProduct>(path, cancellationToken);

        if (result.IsFailure)
            return JsonSerializer.Serialize(new { error = result.Error.Message });

        return JsonSerializer.Serialize(new { totalProducts = result.Value.Count });
    }
}

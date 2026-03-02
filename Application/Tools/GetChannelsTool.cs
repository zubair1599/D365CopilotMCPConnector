using System.ComponentModel;
using System.Text.Json;
using McpD365Server.Application.Interfaces;
using McpD365Server.Application.Models;
using ModelContextProtocol.Server;

namespace McpD365Server.Application.Tools;

/// <summary>
/// MCP Tool: Get retail channels from D365.
/// </summary>
[McpServerToolType]
public sealed class GetChannelsTool
{
    [McpServerTool(Name = "GetChannels"), Description(
        "List retail channels from Dynamics 365. " +
        "Returns channel ID, name, type, operating unit, and default customer account.")]
    public static async Task<string> GetChannelsAsync(
        ID365Client d365Client,
        CancellationToken cancellationToken)
    {
        var path = "data/RetailChannels?$select=ChannelId,ChannelName,ChannelType,OperatingUnitNumber,DefaultCustomerAccount&$top=10";
        var result = await d365Client.GetAsync<RetailChannel>(path, cancellationToken);

        if (result.IsFailure)
            return JsonSerializer.Serialize(new { error = result.Error.Message });

        return JsonSerializer.Serialize(new
        {
            count = result.Value.Value.Length,
            channels = result.Value.Value
        });
    }
}

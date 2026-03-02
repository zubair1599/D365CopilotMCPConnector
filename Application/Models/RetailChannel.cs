using System.Text.Json.Serialization;

namespace McpD365Server.Application.Models;

/// <summary>
/// Retail channel from RetailChannels — 5 key properties only.
/// </summary>
public sealed class RetailChannel
{
    [JsonPropertyName("ChannelId")]
    public string ChannelId { get; set; } = string.Empty;

    [JsonPropertyName("ChannelName")]
    public string ChannelName { get; set; } = string.Empty;

    [JsonPropertyName("ChannelType")]
    public string ChannelType { get; set; } = string.Empty;

    [JsonPropertyName("OperatingUnitNumber")]
    public string OperatingUnitNumber { get; set; } = string.Empty;

    [JsonPropertyName("DefaultCustomerAccount")]
    public string DefaultCustomerAccount { get; set; } = string.Empty;
}

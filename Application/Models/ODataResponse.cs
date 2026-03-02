using System.Text.Json.Serialization;

namespace McpD365Server.Application.Models;

/// <summary>
/// Generic OData response wrapper for D365 entity collections.
/// </summary>
public sealed class ODataResponse<T>
{
    [JsonPropertyName("value")]
    public T[] Value { get; set; } = [];
}

/// <summary>
/// OData response wrapper that includes @odata.count for $count=true queries.
/// </summary>
public sealed class ODataCountResponse<T>
{
    [JsonPropertyName("@odata.count")]
    public int Count { get; set; }

    [JsonPropertyName("value")]
    public T[] Value { get; set; } = [];
}

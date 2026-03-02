using System.Text.Json.Serialization;

namespace McpD365Server.Application.Models;

/// <summary>
/// Sales order header from SalesOrderHeadersV2 — exact D365 OData entity.
/// </summary>
public sealed class SalesOrder
{
    [JsonPropertyName("SalesOrderNumber")]
    public string SalesOrderNumber { get; set; } = string.Empty;

    [JsonPropertyName("OrderCreationDateTime")]
    public string OrderCreationDateTime { get; set; } = string.Empty;

    [JsonPropertyName("SalesOrderStatus")]
    public string SalesOrderStatus { get; set; } = string.Empty;

    [JsonPropertyName("CurrencyCode")]
    public string CurrencyCode { get; set; } = string.Empty;

    [JsonPropertyName("InvoiceCustomerAccountNumber")]
    public string InvoiceCustomerAccountNumber { get; set; } = string.Empty;
}

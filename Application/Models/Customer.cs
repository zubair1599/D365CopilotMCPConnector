using System.Text.Json.Serialization;

namespace McpD365Server.Application.Models;

/// <summary>
/// Customer from CustomersV3 — exact D365 OData schema.
/// </summary>
public sealed class Customer
{
    [JsonPropertyName("CustomerAccount")]
    public string CustomerAccount { get; set; } = string.Empty;

    [JsonPropertyName("OrganizationName")]
    public string OrganizationName { get; set; } = string.Empty;

    [JsonPropertyName("CustomerGroupId")]
    public string CustomerGroupId { get; set; } = string.Empty;

    [JsonPropertyName("SalesCurrencyCode")]
    public string SalesCurrencyCode { get; set; } = string.Empty;

    [JsonPropertyName("InvoiceAccount")]
    public string InvoiceAccount { get; set; } = string.Empty;
}

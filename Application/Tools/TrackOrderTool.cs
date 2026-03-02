using System.ComponentModel;
using System.Text.Json;
using McpD365Server.Application.Interfaces;
using McpD365Server.Application.Models;
using ModelContextProtocol.Server;

namespace McpD365Server.Application.Tools;

/// <summary>
/// MCP Tool: Track a sales order by order number.
/// D365 OData entity: SalesOrderHeadersV2
/// </summary>
[McpServerToolType]
public sealed class TrackOrderTool
{
    [McpServerTool(Name = "TrackOrder"), Description(
        "Track a sales order in Dynamics 365 by its order number. " +
        "Returns order number, creation date, status, currency, and customer account.")]
    public static async Task<string> TrackOrderAsync(
        [Description("The sales order number to track (e.g. 'SO-000001').")] string orderNumber,
        ID365Client d365Client,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(orderNumber))
            return JsonSerializer.Serialize(new { error = "The order number is required." });

        if (orderNumber.Contains('\'') || orderNumber.Contains(';'))
            return JsonSerializer.Serialize(new { error = "Invalid characters in order number." });

        var path = $"data/SalesOrderHeadersV2?$filter=SalesOrderNumber eq '{orderNumber}'" +
                   "&$select=SalesOrderNumber,OrderCreationDateTime,SalesOrderStatus,CurrencyCode,InvoiceCustomerAccountNumber&$top=10";
        var result = await d365Client.GetAsync<SalesOrder>(path, cancellationToken);

        if (result.IsFailure)
            return JsonSerializer.Serialize(new { error = result.Error.Message });

        if (result.Value.Value.Length == 0)
            return JsonSerializer.Serialize(new { error = $"No order found with number '{orderNumber}'." });

        return JsonSerializer.Serialize(new
        {
            count = result.Value.Value.Length,
            orders = result.Value.Value.Select(o => new
            {
                orderNumber = o.SalesOrderNumber,
                createdOn = o.OrderCreationDateTime,
                status = o.SalesOrderStatus,
                currency = o.CurrencyCode,
                customerAccount = o.InvoiceCustomerAccountNumber
            })
        });
    }
}

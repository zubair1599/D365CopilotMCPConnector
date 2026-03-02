using System.ComponentModel;
using System.Text.Json;
using McpD365Server.Application.Interfaces;
using McpD365Server.Application.Models;
using ModelContextProtocol.Server;

namespace McpD365Server.Application.Tools;

/// <summary>
/// MCP Tool: Search customers by account number or organization name.
/// </summary>
[McpServerToolType]
public sealed class FindCustomerTool
{
    [McpServerTool(Name = "FindCustomer"), Description(
        "Search for a customer in Dynamics 365 by customer account number or organization name. " +
        "Provide the customer account (e.g. '004011') or the organization name (e.g. 'Contoso'). " +
        "The system searches both fields automatically. " +
        "Returns customer account, name, group, currency, and invoice account.")]
    public static async Task<string> FindCustomerAsync(
        [Description("The customer account number or organization name to search for.")] string search,
        ID365Client d365Client,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(search))
            return JsonSerializer.Serialize(new { error = "A customer account or organization name is required." });

        if (search.Contains('\'') || search.Contains(';'))
            return JsonSerializer.Serialize(new { error = "Invalid characters in search value." });

        // Search both fields: CustomerAccount OR OrganizationName
        var filter = $"$filter=CustomerAccount eq '{search}' or OrganizationName eq '{search}'";

        var path = $"data/CustomersV3?{filter}" +
                   "&$select=CustomerAccount,OrganizationName,CustomerGroupId,SalesCurrencyCode,InvoiceAccount&$top=10";
        var result = await d365Client.GetAsync<Customer>(path, cancellationToken);

        if (result.IsFailure)
            return JsonSerializer.Serialize(new { error = result.Error.Message });

        if (result.Value.Value.Length == 0)
            return JsonSerializer.Serialize(new { error = $"No customer found matching '{search}'." });

        return JsonSerializer.Serialize(new
        {
            count = result.Value.Value.Length,
            customers = result.Value.Value.Select(c => new
            {
                customerAccount = c.CustomerAccount,
                organizationName = c.OrganizationName,
                customerGroup = c.CustomerGroupId,
                currency = c.SalesCurrencyCode,
                invoiceAccount = c.InvoiceAccount
            })
        });
    }
}

using System.ComponentModel;
using ModelContextProtocol.Server;

namespace McpD365Server.Application.Resources;

/// <summary>
/// MCP Resources: Exposes D365 entity information to Copilot Studio.
/// Resources provide context that helps Copilot Studio understand
/// what data is available and how to use the tools effectively.
/// </summary>
[McpServerResourceType]
public static class D365Resources
{
    [McpServerResource(
        UriTemplate = "d365://entities/overview",
        Name = "D365 Entity Overview",
        MimeType = "text/plain"),
     Description("Overview of available Dynamics 365 OData entities and their purposes.")]
    public static string GetEntityOverview()
    {
        return """
            Available Dynamics 365 Entities:

            1. ReleasedProductsV2 - Released products with number, name, type, price, and status.
            2. InventoryOnHandInventoryV2 - Inventory on-hand with item ID, quantity, and site.
            3. PriceDiscAdmTrans - Price and discount records with amount, currency, and dates.
            4. RetailChannels - Retail channels with ID, name, type, and operating unit.
            5. CustomersV3 - Customers with account, organization name, group, and currency.

            All searches use exact match (eq) filters for maximum OData compatibility.
            List results are limited to 10 items by default.
            """;
    }

    [McpServerResource(
        UriTemplate = "d365://tools/guide",
        Name = "Tool Usage Guide",
        MimeType = "text/plain"),
     Description("Guide on how to use the available D365 tools effectively.")]
    public static string GetToolGuide()
    {
        return """
            Tool Usage Guide:

            FindProduct - Search by exact ProductNumber. Example: 'D0001'
            CheckInventory - Check stock by ItemId. Supports batch: 'ITEM001,ITEM002'
            GetProductPrice - Look up all prices for a product by ItemRelation.
            GetActiveSalesPrice - Same as above but filtered to currently active prices only.
            CountProducts - Returns total number of released products. No parameters needed.
            GetProductStatus - Check if a product is Active, Inactive, or Blocked.
            GetChannels - List retail channels (stores, online). Returns top 10.
            FindCustomer - Search by exact CustomerAccount or OrganizationName.
            """;
    }
}

using System.Text.Json.Serialization;
using McpD365Server.Application.Models;

namespace McpD365Server.Infrastructure.Serialization;

/// <summary>
/// Source-generated JSON serializer context for all D365 models.
/// Eliminates runtime reflection for high-throughput, low-allocation serialization.
/// </summary>
[JsonSerializable(typeof(ODataResponse<InventoryOnHand>))]
[JsonSerializable(typeof(ODataResponse<ReleasedProduct>))]
[JsonSerializable(typeof(ODataResponse<PriceRecord>))]
[JsonSerializable(typeof(ODataResponse<RetailChannel>))]
[JsonSerializable(typeof(ODataResponse<Customer>))]
[JsonSerializable(typeof(ODataResponse<SalesOrder>))]
[JsonSerializable(typeof(ODataCountResponse<ReleasedProduct>))]
[JsonSerializable(typeof(InventoryOnHand[]))]
[JsonSerializable(typeof(ReleasedProduct[]))]
[JsonSerializable(typeof(PriceRecord[]))]
[JsonSerializable(typeof(RetailChannel[]))]
[JsonSerializable(typeof(Customer[]))]
[JsonSerializable(typeof(SalesOrder[]))]
[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    WriteIndented = false)]
public partial class D365JsonContext : JsonSerializerContext;

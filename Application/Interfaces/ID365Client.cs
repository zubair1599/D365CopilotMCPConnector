using McpD365Server.Application.Models;
using McpD365Server.Domain;

namespace McpD365Server.Application.Interfaces;

/// <summary>
/// Abstraction for Dynamics 365 OData HTTP communication.
/// </summary>
public interface ID365Client
{
    /// <summary>
    /// Executes a GET request against a D365 OData entity path and deserializes the response.
    /// </summary>
    Task<Result<ODataResponse<T>>> GetAsync<T>(string odataPath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a GET request with $count=true and returns the count along with the data.
    /// </summary>
    Task<Result<ODataCountResponse<T>>> GetWithCountAsync<T>(string odataPath, CancellationToken cancellationToken = default);
}

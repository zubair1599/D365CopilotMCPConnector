using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using McpD365Server.Application.Interfaces;
using McpD365Server.Application.Models;
using McpD365Server.Domain;
using McpD365Server.Infrastructure.Serialization;
using Microsoft.Extensions.Logging;

namespace McpD365Server.Infrastructure;

/// <summary>
/// Typed HttpClient for Dynamics 365 OData communication.
/// Forwards the incoming bearer token from the MCP request to D365.
/// Uses source-generated JSON serialization and the Result pattern.
/// All exceptions are caught and returned as Result.Fail — NO exceptions escape this class.
/// </summary>
public sealed class D365Client : ID365Client
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<D365Client> _logger;
    private static readonly JsonSerializerOptions s_jsonOptions = D365JsonContext.Default.Options;

    public D365Client(
        HttpClient httpClient,
        IHttpContextAccessor httpContextAccessor,
        ILogger<D365Client> logger)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<Result<ODataResponse<T>>> GetAsync<T>(
        string odataPath,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("D365 GET: {Path}", odataPath);

        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, odataPath);
            AttachBearerToken(request);

            var response = await _httpClient.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized ||
                response.StatusCode == HttpStatusCode.Forbidden)
            {
                _logger.LogWarning("D365 returned {StatusCode} — token may be invalid or expired.", response.StatusCode);
                return Result.Fail<ODataResponse<T>>(DomainErrors.D365.Unauthorized);
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogWarning("D365 request failed [{StatusCode}]: {Body}",
                    (int)response.StatusCode, errorBody);
                return Result.Fail<ODataResponse<T>>(
                    DomainErrors.D365.RequestFailed((int)response.StatusCode, errorBody));
            }

            var result = await response.Content.ReadFromJsonAsync<ODataResponse<T>>(
                s_jsonOptions, cancellationToken);

            if (result is null)
            {
                _logger.LogWarning("D365 deserialization returned null for {Path}", odataPath);
                return Result.Fail<ODataResponse<T>>(DomainErrors.D365.DeserializationFailed);
            }

            return Result.Success(result);
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            _logger.LogError(ex, "D365 request timed out for {Path}", odataPath);
            return Result.Fail<ODataResponse<T>>(DomainErrors.D365.Timeout);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "D365 connection error for {Path}", odataPath);
            return Result.Fail<ODataResponse<T>>(DomainErrors.D365.ConnectionFailed);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error calling D365 {Path}", odataPath);
            return Result.Fail<ODataResponse<T>>(DomainErrors.General.InternalError);
        }
    }

    /// <inheritdoc />
    public async Task<Result<ODataCountResponse<T>>> GetWithCountAsync<T>(
        string odataPath,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("D365 GET (with count): {Path}", odataPath);

        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, odataPath);
            AttachBearerToken(request);

            var response = await _httpClient.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized ||
                response.StatusCode == HttpStatusCode.Forbidden)
            {
                _logger.LogWarning("D365 returned {StatusCode} — token may be invalid or expired.", response.StatusCode);
                return Result.Fail<ODataCountResponse<T>>(DomainErrors.D365.Unauthorized);
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogWarning("D365 request failed [{StatusCode}]: {Body}",
                    (int)response.StatusCode, errorBody);
                return Result.Fail<ODataCountResponse<T>>(
                    DomainErrors.D365.RequestFailed((int)response.StatusCode, errorBody));
            }

            var result = await response.Content.ReadFromJsonAsync<ODataCountResponse<T>>(
                s_jsonOptions, cancellationToken);

            if (result is null)
            {
                _logger.LogWarning("D365 deserialization returned null for {Path}", odataPath);
                return Result.Fail<ODataCountResponse<T>>(DomainErrors.D365.DeserializationFailed);
            }

            return Result.Success(result);
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            _logger.LogError(ex, "D365 request timed out for {Path}", odataPath);
            return Result.Fail<ODataCountResponse<T>>(DomainErrors.D365.Timeout);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "D365 connection error for {Path}", odataPath);
            return Result.Fail<ODataCountResponse<T>>(DomainErrors.D365.ConnectionFailed);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error calling D365 {Path}", odataPath);
            return Result.Fail<ODataCountResponse<T>>(DomainErrors.General.InternalError);
        }
    }

    /// <summary>
    /// Extracts the bearer token from the incoming MCP HTTP request
    /// and forwards it to the outgoing D365 OData request.
    /// Copilot Studio → MCP Server (token received) → D365 OData (token forwarded).
    /// </summary>
    private void AttachBearerToken(HttpRequestMessage request)
    {
        var authHeader = _httpContextAccessor.HttpContext?
            .Request.Headers.Authorization.ToString();

        if (!string.IsNullOrWhiteSpace(authHeader) &&
            authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            var token = authHeader["Bearer ".Length..].Trim();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            _logger.LogDebug("Bearer token forwarded to D365.");
        }
        else
        {
            _logger.LogWarning("No bearer token found on incoming request to forward to D365.");
        }
    }
}

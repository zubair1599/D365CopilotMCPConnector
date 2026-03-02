using McpD365Server.Application.Interfaces;
using McpD365Server.Infrastructure;
using McpD365Server.Infrastructure.Resilience;

namespace McpD365Server.Configuration;

/// <summary>
/// Centralized dependency injection configuration.
/// Registers all application services, typed HttpClients, and resilience policies.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds all application services to the DI container.
    /// </summary>
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var d365BaseUrl = configuration["D365BaseUrl"]
            ?? throw new InvalidOperationException("D365BaseUrl is not configured in appsettings.json.");

        // Required for D365Client to access the incoming HTTP request's bearer token
        services.AddHttpContextAccessor();

        // Register D365Client as a typed HttpClient with Polly resilience
        services.AddHttpClient<ID365Client, D365Client>(client =>
        {
            client.BaseAddress = new Uri(d365BaseUrl.TrimEnd('/') + "/");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
            client.DefaultRequestHeaders.Add("OData-Version", "4.0");
            client.Timeout = TimeSpan.FromSeconds(60);
        })
        .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
        {
            // Connection pooling for high performance
            PooledConnectionLifetime = TimeSpan.FromMinutes(5),
            PooledConnectionIdleTimeout = TimeSpan.FromMinutes(2),
            MaxConnectionsPerServer = 20,
            EnableMultipleHttp2Connections = true
        })
        .AddD365ResiliencePipeline(); // Polly retry + circuit breaker + timeout

        return services;
    }
}

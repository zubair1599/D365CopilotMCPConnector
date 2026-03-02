namespace McpD365Server.Host;

/// <summary>
/// MCP endpoint registration extensions.
/// </summary>
public static class McpEndpointRegistration
{
    /// <summary>
    /// Maps MCP and health check endpoints.
    /// The MCP SDK handles GET (SSE stream), POST (JSON-RPC), and DELETE (session close)
    /// automatically on the same /mcp route.
    /// </summary>
    public static WebApplication MapMcpEndpoints(this WebApplication app)
    {
        // Map MCP at /mcp with JWT authorization enforced.
        // GET  /mcp → SSE stream for server-to-client notifications
        // POST /mcp → JSON-RPC requests (initialize, tools/call, tools/list, etc.)
        // DELETE /mcp → session termination
        app.MapMcp("/mcp")
           .RequireAuthorization();

        // Health check endpoint for production monitoring (no auth required)
        app.MapGet("/health", () => Results.Ok(new
        {
            status = "healthy",
            service = "McpD365Server",
            timestamp = DateTime.UtcNow
        }));

        return app;
    }
}

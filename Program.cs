using McpD365Server.Configuration;
using McpD365Server.Host;
using McpD365Server.Infrastructure.Auth;

var builder = WebApplication.CreateBuilder(args);

// ── Authentication: Validate Copilot Studio JWT via Microsoft public keys ──
builder.Services.AddEntraIdAuthentication();

// ── Application Services: HttpClient, D365Client, DI ──
builder.Services.AddApplicationServices(builder.Configuration);

// ── MCP Server: Register MCP with HTTP transport + auto-discover tools ──
builder.Services
    .AddMcpServer()
    .WithHttpTransport()
    .WithToolsFromAssembly();

// ── Build ──
var app = builder.Build();

// ── Global Exception Handler ──
// Returns clean JSON-RPC errors — never exposes stack traces or internal details.
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status400BadRequest;

        var errorResponse = new
        {
            jsonrpc = "2.0",
            error = new
            {
                code = -32700,
                message = "The request could not be processed. Please ensure the request body is valid JSON and all required headers are included."
            },
            id = (string?)null
        };

        await context.Response.WriteAsJsonAsync(errorResponse);
    });
});

// ── Middleware Pipeline ──
app.UseAuthentication();
app.UseAuthorization();

// ── Map MCP endpoint at /mcp with authorization ──
app.MapMcpEndpoints();

// ── Run ──
app.Run();

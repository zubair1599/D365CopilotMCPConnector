using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace McpD365Server.Infrastructure.Auth;

/// <summary>
/// Extension methods for configuring JWT Bearer authentication.
/// Validates incoming tokens from Copilot Studio against Microsoft Entra ID
/// public signing keys WITHOUT requiring a pre-configured client ID or secret.
///
/// Flow: Copilot Studio sends a JWT → MCP Server validates signature, expiration,
/// and issuer using Microsoft's OIDC metadata endpoint → extracts tenant/client from claims.
/// </summary>
public static class EntraIdAuthExtensions
{
    /// <summary>
    /// Microsoft Entra ID common metadata endpoint for multi-tenant token validation.
    /// Uses the "common" authority so tokens from ANY Entra ID tenant are validated
    /// against Microsoft's public signing keys.
    /// </summary>
    private const string EntraIdMetadataEndpoint =
        "https://login.microsoftonline.com/common/v2.0/.well-known/openid-configuration";

    /// <summary>
    /// Adds JWT Bearer authentication that validates tokens from Copilot Studio.
    /// No client ID or client secret is needed on the server side.
    /// The server fetches Microsoft's public signing keys automatically and validates:
    /// - Token signature (RSA via OIDC JWKS)
    /// - Token expiration
    /// - Token issuer (must be a valid Microsoft Entra ID issuer)
    /// </summary>
    public static IServiceCollection AddEntraIdAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                // Use Microsoft's common OIDC metadata to discover signing keys
                options.MetadataAddress = EntraIdMetadataEndpoint;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // Validate the token was signed by Microsoft's keys
                    ValidateIssuerSigningKey = true,

                    // Validate expiration
                    ValidateLifetime = true,
                    RequireExpirationTime = true,

                    // Accept tokens from any Microsoft Entra ID tenant
                    // The issuer format is: https://login.microsoftonline.com/{tenantId}/v2.0
                    // or https://sts.windows.net/{tenantId}/
                    ValidateIssuer = true,
                    IssuerValidator = ValidateMicrosoftIssuer,

                    // Do NOT validate audience — we don't have a pre-configured client ID.
                    // The audience (aud claim) in the token is the Copilot Studio app's client ID.
                    // We trust any token validly signed by Microsoft Entra ID.
                    ValidateAudience = false,

                    // Minimal clock skew for strict expiration
                    ClockSkew = TimeSpan.FromMinutes(2),

                    // Require signed tokens
                    RequireSignedTokens = true,
                };

                // Optional: log auth failures for debugging
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        var logger = context.HttpContext.RequestServices
                            .GetRequiredService<ILoggerFactory>()
                            .CreateLogger("EntraIdAuth");

                        logger.LogWarning(context.Exception,
                            "JWT authentication failed: {Message}", context.Exception.Message);

                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        var logger = context.HttpContext.RequestServices
                            .GetRequiredService<ILoggerFactory>()
                            .CreateLogger("EntraIdAuth");

                        var tenantId = context.Principal?.FindFirst("tid")?.Value ?? "unknown";
                        var appId = context.Principal?.FindFirst("appid")?.Value
                                 ?? context.Principal?.FindFirst("azp")?.Value
                                 ?? "unknown";

                        logger.LogInformation(
                            "Token validated. TenantId: {TenantId}, AppId: {AppId}",
                            tenantId, appId);

                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorization();

        return services;
    }

    /// <summary>
    /// Custom issuer validator that accepts any valid Microsoft Entra ID issuer.
    /// Entra ID tokens use issuer formats:
    ///   - https://login.microsoftonline.com/{tenantId}/v2.0  (v2.0 tokens)
    ///   - https://sts.windows.net/{tenantId}/                (v1.0 tokens)
    /// </summary>
    private static string ValidateMicrosoftIssuer(
        string issuer,
        SecurityToken securityToken,
        TokenValidationParameters validationParameters)
    {
        if (issuer.StartsWith("https://login.microsoftonline.com/", StringComparison.OrdinalIgnoreCase) ||
            issuer.StartsWith("https://sts.windows.net/", StringComparison.OrdinalIgnoreCase))
        {
            return issuer;
        }

        throw new SecurityTokenInvalidIssuerException(
            $"Token issuer '{issuer}' is not a recognized Microsoft Entra ID issuer.");
    }
}

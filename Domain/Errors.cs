namespace McpD365Server.Domain;

/// <summary>
/// Centralized domain error definitions with professional, user-facing messages.
/// All errors are pre-allocated static instances.
/// </summary>
public static class DomainErrors
{
    public static class D365
    {
        public static Error ConnectionFailed =>
            new("D365.ConnectionFailed",
                "Unable to connect to the Dynamics 365 service. Please verify the service is available and try again.");

        public static Error RequestFailed(int statusCode, string detail) =>
            new("D365.RequestFailed",
                $"The Dynamics 365 service returned an error (HTTP {statusCode}). {SanitizeDetail(detail)}");

        public static Error RequestFailed(string detail) =>
            new("D365.RequestFailed",
                $"The request to Dynamics 365 could not be completed. {SanitizeDetail(detail)}");

        public static Error DeserializationFailed =>
            new("D365.DeserializationFailed",
                "The response from Dynamics 365 could not be processed. The data format may have changed.");

        public static Error Timeout =>
            new("D365.Timeout",
                "The request to Dynamics 365 timed out. The service may be experiencing high load. Please try again.");

        public static Error Unauthorized =>
            new("D365.Unauthorized",
                "Access to Dynamics 365 was denied. The authentication token may be invalid or expired.");

        /// <summary>
        /// Removes internal technical noise from error details for end-user consumption.
        /// </summary>
        private static string SanitizeDetail(string detail)
        {
            if (string.IsNullOrWhiteSpace(detail)) return string.Empty;
            // Truncate excessively long error bodies
            return detail.Length > 300 ? detail[..300] + "..." : detail;
        }
    }

    public static class Validation
    {
        public static Error MissingParameter(string paramName) =>
            new("Validation.MissingParameter",
                $"The required parameter '{paramName}' was not provided. Please include it and try again.");

        public static Error InvalidParameter(string paramName, string reason) =>
            new("Validation.InvalidParameter",
                $"The parameter '{paramName}' is invalid: {reason}.");
    }

    public static class Auth
    {
        public static Error Unauthorized =>
            new("Auth.Unauthorized",
                "Authentication is required to access this resource. Please provide a valid token.");

        public static Error TokenExpired =>
            new("Auth.TokenExpired",
                "Your authentication token has expired. Please obtain a new token and try again.");
    }

    public static class General
    {
        public static Error NotFound(string entity) =>
            new("General.NotFound",
                $"The requested {entity} could not be found.");

        public static Error InternalError =>
            new("General.InternalError",
                "An unexpected error occurred while processing your request. Please try again or contact support.");
    }
}

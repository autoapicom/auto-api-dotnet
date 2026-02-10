namespace AutoApi;

/// <summary>
/// Base exception for all auto-api.com API errors.
/// </summary>
public class ApiException : Exception
{
    /// <summary>
    /// HTTP status code returned by the API.
    /// </summary>
    public int StatusCode { get; }

    /// <summary>
    /// Raw response body from the API.
    /// </summary>
    public string ResponseBody { get; }

    public ApiException(int statusCode, string message, string responseBody)
        : base(message)
    {
        StatusCode = statusCode;
        ResponseBody = responseBody;
    }
}

/// <summary>
/// Exception for authentication errors (HTTP 401/403).
/// </summary>
public class AuthException : ApiException
{
    public AuthException(int statusCode, string message, string responseBody)
        : base(statusCode, message, responseBody) { }
}

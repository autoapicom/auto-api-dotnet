using Xunit;

namespace AutoApi.Tests;

public class ExceptionTests
{
    // ── ApiException ────────────────────────────────────────────

    [Fact]
    public void ApiException_ExtendsException()
    {
        var ex = new ApiException(500, "Error", "{}");
        Assert.IsAssignableFrom<Exception>(ex);
    }

    [Fact]
    public void ApiException_StoresMessage()
    {
        var ex = new ApiException(500, "Something went wrong", "{}");
        Assert.Equal("Something went wrong", ex.Message);
    }

    [Fact]
    public void ApiException_StoresStatusCode()
    {
        var ex = new ApiException(422, "Error", "{}");
        Assert.Equal(422, ex.StatusCode);
    }

    [Fact]
    public void ApiException_StoresResponseBody()
    {
        var body = """{"message":"Validation failed"}""";
        var ex = new ApiException(422, "Error", body);
        Assert.Equal(body, ex.ResponseBody);
    }

    // ── AuthException ───────────────────────────────────────────

    [Fact]
    public void AuthException_ExtendsApiException()
    {
        var ex = new AuthException(401, "Unauthorized", "{}");
        Assert.IsAssignableFrom<ApiException>(ex);
    }

    [Fact]
    public void AuthException_StoresStatusCode()
    {
        var ex = new AuthException(403, "Forbidden", "{}");
        Assert.Equal(403, ex.StatusCode);
    }

    [Fact]
    public void AuthException_StoresMessage()
    {
        var ex = new AuthException(401, "Access denied", "{}");
        Assert.Equal("Access denied", ex.Message);
    }

    [Fact]
    public void AuthException_StoresResponseBody()
    {
        var body = """{"message":"Bad key"}""";
        var ex = new AuthException(401, "Bad key", body);
        Assert.Equal(body, ex.ResponseBody);
    }
}

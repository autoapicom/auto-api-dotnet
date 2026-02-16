using System.Net;

namespace AutoApi.Tests;

/// <summary>
/// A test HttpMessageHandler that returns a preconfigured response
/// and captures the last request for assertions.
/// </summary>
public class MockHandler : HttpMessageHandler
{
    private readonly HttpStatusCode _statusCode;
    private readonly string _body;

    public HttpRequestMessage? LastRequest { get; private set; }
    public string? LastRequestBody { get; private set; }

    public MockHandler(string body, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        _body = body;
        _statusCode = statusCode;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        LastRequest = request;

        if (request.Content != null)
        {
            LastRequestBody = await request.Content.ReadAsStringAsync(cancellationToken);
        }

        return new HttpResponseMessage(_statusCode)
        {
            Content = new StringContent(_body, System.Text.Encoding.UTF8, "application/json"),
        };
    }
}

using System.Net.Http.Headers;
using System.Text;
using Gateway.Api.Options;
using Microsoft.Extensions.Options;

namespace Gateway.Api.Proxy;

public sealed class GatewayProxyExecutor(
    IHttpClientFactory httpClientFactory,
    IOptions<GatewayOptions> options)
{
    public async Task ProxyAsync(
        HttpContext context,
        string serviceName,
        string? path,
        CancellationToken cancellationToken)
    {
        if (!options.Value.Services.TryGetValue(serviceName, out var baseUrl))
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsJsonAsync(new { error = "unknown-service", serviceName }, cancellationToken);
            return;
        }

        var targetUri = BuildTargetUri(baseUrl, path, context.Request.QueryString.Value);
        if (targetUri is null)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new { error = "invalid-target-uri" }, cancellationToken);
            return;
        }

        using var requestMessage = new HttpRequestMessage(new HttpMethod(context.Request.Method), targetUri);
        CopyRequestHeaders(context, requestMessage);

        if (context.Request.ContentLength is > 0 || context.Request.Headers.ContainsKey("Transfer-Encoding"))
        {
            requestMessage.Content = new StreamContent(context.Request.Body);
            if (!string.IsNullOrWhiteSpace(context.Request.ContentType))
            {
                requestMessage.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(context.Request.ContentType);
            }
        }

        var client = httpClientFactory.CreateClient();
        using var responseMessage = await client.SendAsync(
            requestMessage,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);

        context.Response.StatusCode = (int)responseMessage.StatusCode;
        CopyResponseHeaders(responseMessage, context.Response);

        await using var responseStream = await responseMessage.Content.ReadAsStreamAsync(cancellationToken);
        await responseStream.CopyToAsync(context.Response.Body, cancellationToken);
    }

    private static Uri? BuildTargetUri(string baseUrl, string? path, string? queryString)
    {
        var trimmedBase = baseUrl.TrimEnd('/');
        var trimmedPath = string.IsNullOrWhiteSpace(path) ? string.Empty : path.TrimStart('/');
        var target = string.IsNullOrEmpty(trimmedPath) ? trimmedBase : $"{trimmedBase}/{trimmedPath}";

        if (!string.IsNullOrWhiteSpace(queryString))
        {
            target += queryString;
        }

        return Uri.TryCreate(target, UriKind.Absolute, out var uri) ? uri : null;
    }

    private static void CopyRequestHeaders(HttpContext context, HttpRequestMessage requestMessage)
    {
        foreach (var header in context.Request.Headers)
        {
            if (ShouldSkipRequestHeader(header.Key))
            {
                continue;
            }

            if (!requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray()))
            {
                requestMessage.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
            }
        }
    }

    private static void CopyResponseHeaders(HttpResponseMessage source, HttpResponse destination)
    {
        foreach (var header in source.Headers)
        {
            if (ShouldSkipResponseHeader(header.Key))
            {
                continue;
            }

            destination.Headers[header.Key] = header.Value.ToArray();
        }

        foreach (var header in source.Content.Headers)
        {
            if (ShouldSkipResponseHeader(header.Key))
            {
                continue;
            }

            destination.Headers[header.Key] = header.Value.ToArray();
        }
    }

    private static bool ShouldSkipRequestHeader(string headerName) =>
        string.Equals(headerName, "Host", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(headerName, "Content-Length", StringComparison.OrdinalIgnoreCase);

    private static bool ShouldSkipResponseHeader(string headerName) =>
        string.Equals(headerName, "Transfer-Encoding", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(headerName, "Connection", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(headerName, "Keep-Alive", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(headerName, "Proxy-Authenticate", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(headerName, "Proxy-Authorization", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(headerName, "TE", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(headerName, "Trailer", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(headerName, "Upgrade", StringComparison.OrdinalIgnoreCase);
}

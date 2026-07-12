using System.Net.Http.Headers;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient();

var app = builder.Build();

var serviceMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
{
    ["auth"] = Environment.GetEnvironmentVariable("AUTH_SERVICE_URL") ?? "http://localhost:8001",
    ["catalog"] = Environment.GetEnvironmentVariable("CATALOG_SERVICE_URL") ?? "http://localhost:8002",
    ["order"] = Environment.GetEnvironmentVariable("ORDER_SERVICE_URL") ?? "http://localhost:8003",
    ["inventory"] = Environment.GetEnvironmentVariable("INVENTORY_SERVICE_URL") ?? "http://localhost:8004",
    ["notification"] = Environment.GetEnvironmentVariable("NOTIFICATION_SERVICE_URL") ?? "http://localhost:8005",
};

app.MapGet("/", () => Results.Ok(new
{
    service = Environment.GetEnvironmentVariable("SERVICE_NAME") ?? "gateway",
    status = "ok",
}));

app.MapGet("/health", () => Results.Ok(new
{
    service = Environment.GetEnvironmentVariable("SERVICE_NAME") ?? "gateway",
    status = "ok",
}));

app.MapGet("/services", () => Results.Ok(new
{
    service = Environment.GetEnvironmentVariable("SERVICE_NAME") ?? "gateway",
    downstream = serviceMap,
}));

app.MapMethods("/proxy/{serviceName}/{**path}", new[] { "GET", "POST", "PUT", "PATCH", "DELETE" }, ProxyAsync);

app.Run();

async Task<IResult> ProxyAsync(
    HttpContext context,
    string serviceName,
    string? path,
    IHttpClientFactory httpClientFactory)
{
    if (!serviceMap.TryGetValue(serviceName, out var baseUrl))
    {
        return Results.NotFound(new { error = "unknown-service", serviceName });
    }

    var targetUri = BuildTargetUri(baseUrl, path, context.Request.QueryString.Value);
    if (targetUri is null)
    {
        return Results.BadRequest(new { error = "invalid-target-uri" });
    }

    var requestMessage = new HttpRequestMessage(new HttpMethod(context.Request.Method), targetUri);

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
    using var responseMessage = await client.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, context.RequestAborted);
    var responseBody = await responseMessage.Content.ReadAsStringAsync(context.RequestAborted);
    var contentType = responseMessage.Content.Headers.ContentType?.ToString() ?? "application/json";

    return Results.Content(responseBody, contentType, Encoding.UTF8, (int)responseMessage.StatusCode);
}

static Uri? BuildTargetUri(string baseUrl, string? path, string? queryString)
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

static void CopyRequestHeaders(HttpContext context, HttpRequestMessage requestMessage)
{
    foreach (var header in context.Request.Headers)
    {
        if (string.Equals(header.Key, "Host", StringComparison.OrdinalIgnoreCase))
        {
            continue;
        }

        if (!requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray()))
        {
            requestMessage.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
        }
    }
}

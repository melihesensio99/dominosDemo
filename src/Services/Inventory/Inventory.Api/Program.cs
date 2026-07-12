using System.Collections.Concurrent;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var stock = new ConcurrentDictionary<string, InventoryState>(new[]
{
    new KeyValuePair<string, InventoryState>("p-100", new InventoryState(25, 0)),
    new KeyValuePair<string, InventoryState>("p-200", new InventoryState(12, 0)),
    new KeyValuePair<string, InventoryState>("p-300", new InventoryState(5, 0)),
});

app.MapGet("/", () => Results.Ok(new
{
    service = Environment.GetEnvironmentVariable("SERVICE_NAME") ?? "inventory",
    status = "ok",
}));

app.MapGet("/health", () => Results.Ok(new
{
    service = Environment.GetEnvironmentVariable("SERVICE_NAME") ?? "inventory",
    status = "ok",
}));

app.MapGet("/stock/{productId}", (string productId) =>
{
    return stock.TryGetValue(productId, out var state)
        ? Results.Ok(new { productId, state.Available, state.Reserved })
        : Results.Ok(new { productId, available = 0, reserved = 0 });
});

app.MapPost("/stock/{productId}/reserve", (string productId, ReserveStockRequest request) =>
{
    return stock.AddOrUpdate(
        productId,
        _ => new InventoryState(0, request.Quantity),
        (_, current) =>
        {
            var availableToReserve = Math.Max(0, current.Available - request.Quantity);
            var reserved = current.Reserved + Math.Min(current.Available, request.Quantity);
            return current with { Available = availableToReserve, Reserved = reserved };
        });
});

app.Run();

internal sealed record InventoryState(int Available, int Reserved);

internal sealed record ReserveStockRequest(int Quantity);

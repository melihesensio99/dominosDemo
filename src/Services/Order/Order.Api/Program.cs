using System.Collections.Concurrent;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var orders = new ConcurrentDictionary<string, OrderDto>();

app.MapGet("/orders", () => Results.Ok(new { items = orders.Values.OrderByDescending(order => order.CreatedAt) }));

app.MapPost("/orders", (CreateOrderRequest request) =>
{
    var id = Guid.NewGuid().ToString("N");
    var order = new OrderDto(
        id,
        request.CustomerId,
        request.Items,
        "pending",
        DateTimeOffset.UtcNow);

    orders[id] = order;
    return Results.Created($"/orders/{id}", order);
});

app.MapGet("/orders/{id}", (string id) =>
{
    return orders.TryGetValue(id, out var order)
        ? Results.Ok(order)
        : Results.NotFound(new { error = "order-not-found", id });
});

app.Run();

internal sealed record OrderItem(string ProductId, int Quantity);

internal sealed record CreateOrderRequest(string CustomerId, List<OrderItem> Items);

internal sealed record OrderDto(
    string Id,
    string CustomerId,
    List<OrderItem> Items,
    string Status,
    DateTimeOffset CreatedAt);

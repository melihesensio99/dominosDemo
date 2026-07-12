var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var products = new[]
{
    new Product("p-100", "Starter Box", 100m, 25),
    new Product("p-200", "Pro Box", 250m, 12),
    new Product("p-300", "Enterprise Box", 500m, 5),
};

app.MapGet("/", () => Results.Ok(new
{
    service = Environment.GetEnvironmentVariable("SERVICE_NAME") ?? "catalog",
    status = "ok",
}));

app.MapGet("/health", () => Results.Ok(new
{
    service = Environment.GetEnvironmentVariable("SERVICE_NAME") ?? "catalog",
    status = "ok",
}));

app.MapGet("/products", () => Results.Ok(new { items = products }));

app.MapGet("/products/{id}", (string id) =>
{
    var product = products.FirstOrDefault(x => x.Id == id);
    return product is null ? Results.NotFound(new { error = "product-not-found", id }) : Results.Ok(product);
});

app.Run();

internal sealed record Product(string Id, string Name, decimal Price, int Stock);

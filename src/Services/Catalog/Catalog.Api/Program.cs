using Catalog.Api.Features;

var builder = WebApplication.CreateBuilder(args);

CatalogModule.ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

app.UseGlobalExceptionHandler();

app.MapCatalogEndpoints();

await app.Services.InitializeCatalogDatabaseAsync();

app.Run();

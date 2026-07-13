using Auth.Api.Features;
using BuildingBlocks.Web;

var builder = WebApplication.CreateBuilder(args);

AuthModule.ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

app.UseGlobalExceptionHandler();

app.MapAuthEndpoints();

await app.Services.InitializeAuthDatabaseAsync();

app.Run();

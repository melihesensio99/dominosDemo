using Auth.Api.Features;
using BuildingBlocks.Web;

var builder = WebApplication.CreateBuilder(args);

AuthModule.ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

app.UseGlobalExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();

app.MapAuthEndpoints();

await app.Services.InitializeAuthDatabaseAsync();

app.Run();

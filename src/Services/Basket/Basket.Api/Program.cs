using Basket.Api.Features;

var builder = WebApplication.CreateBuilder(args);

BasketModule.ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

app.UseGlobalExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();

app.MapBasketEndpoints();

app.Run();

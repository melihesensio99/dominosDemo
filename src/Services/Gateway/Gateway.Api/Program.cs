using Gateway.Api.Extensions;
using Gateway.Api.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<GatewayOptions>(options =>
{
    var serviceMapSection = builder.Configuration.GetSection(GatewayOptions.SectionName);

    foreach (var service in serviceMapSection.GetChildren())
    {
        if (!string.IsNullOrWhiteSpace(service.Value))
        {
            options.Services[service.Key] = service.Value;
        }
    }
});
builder.Services.AddGatewayProxy();
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin();
    });
});

var app = builder.Build();
app.UseCors("Frontend");

app.MapGatewayEndpoints();

app.Run();

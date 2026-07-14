using Order.Api.Features;
using Order.Api.Infrastructure;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

OrderModule.ConfigureServices(builder.Services, builder.Configuration);

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();

    x.UsingRabbitMq((context, cfg) =>
    {
        var host = builder.Configuration["RabbitMq:Host"] ?? "localhost";
        var username = builder.Configuration["RabbitMq:Username"] ?? "guest";
        var password = builder.Configuration["RabbitMq:Password"] ?? "guest";

        cfg.Host(host, "/", h =>
        {
            h.Username(username);
            h.Password(password);
        });
    });
});

var app = builder.Build();

app.UseGlobalExceptionHandler();

app.MapOrderEndpoints();

await app.Services.InitializeOrderDatabaseAsync();

app.Run();

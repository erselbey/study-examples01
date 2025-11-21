using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(cfg =>
{
    MassTransitConfig.Configure(cfg);
    cfg.UsingRabbitMq(MassTransitConfig.ConfigureBus);
});

var app = builder.Build();
app.MapGet("/health", () => Results.Ok("OK"));
app.Run();

using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(cfg =>
{
    cfg.UsingRabbitMq((context, bus) =>
    {
        bus.Host("rabbitmq", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
    });
});

var app = builder.Build();

app.MapPost("/api/orders", async (IPublishEndpoint bus, OrderRequest request) =>
{
    await Task.Delay(300); // DB mock
    var evt = new OrderCreatedEvent(Guid.NewGuid(), request.CustomerEmail, request.Total);
    await bus.Publish(evt);
    return Results.Accepted($"/api/orders/{evt.OrderId}", evt);
});

app.Run();

record OrderRequest(string CustomerEmail, decimal Total);
record OrderCreatedEvent(Guid OrderId, string CustomerEmail, decimal Total);

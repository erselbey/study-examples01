using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(cfg =>
{
    cfg.AddConsumer<OrderCreatedConsumer>();
    cfg.UsingRabbitMq((context, bus) =>
    {
        bus.Host("172-3-10-24.nip.io", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        bus.ReceiveEndpoint("order-created-queue02", endpoint =>
        {
            endpoint.ConfigureConsumer<OrderCreatedConsumer>(context);
        });
    });
});

var app = builder.Build();
app.MapGet("/health", () => Results.Ok("OK"));
app.Run();

class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
{
    private readonly ILogger<OrderCreatedConsumer> _logger;
    public OrderCreatedConsumer(ILogger<OrderCreatedConsumer> logger) => _logger = logger;

    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        await Task.Delay(500);
        _logger.LogInformation("Order {OrderId} processed for {Email}",
            context.Message.OrderId, context.Message.CustomerEmail);
    }
}

namespace Contracts
{
    public record OrderCreatedEvent(Guid OrderId, string CustomerEmail, decimal Total);
}

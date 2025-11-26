using Contracts;
using MassTransit;
using MassTransit.KafkaIntegration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);
var transport = builder.Configuration["TRANSPORT"]?.ToLowerInvariant() ?? "rabbitmq";
var rabbitHost = builder.Configuration["RABBITMQ_HOST"] ?? "rabbitmq";
var kafkaHost = builder.Configuration["KAFKA_HOST"] ?? "kafka";
var kafkaPort = builder.Configuration["KAFKA_PORT"] ?? "9092";

builder.Services.AddMassTransit(cfg =>
{
    cfg.AddConsumer<OrderCreatedConsumer>();

    if (transport == "$")
    {
        cfg.UsingInMemory((context, bus) => bus.ConfigureEndpoints(context));

        cfg.AddRider(rider =>
        {
            rider.AddConsumer<OrderCreatedConsumer>();
            rider.UsingKafka((context, k) =>
            {
                k.Host($"{kafkaHost}:{kafkaPort}");
                k.TopicEndpoint<OrderCreatedEvent>("order-created-01", "notification-service", e =>
                {
                    e.ConfigureConsumer<OrderCreatedConsumer>(context);
                });
            });
        });
    }
    else
    {
        cfg.UsingRabbitMq((context, bus) =>
        {
            bus.Host(rabbitHost, "/", h =>
            {
                h.Username("Admin");
                h.Password("guest");
            });

            bus.ReceiveEndpoint("order-created-queue-01", endpoint =>
            {
                endpoint.ConfigureConsumer<OrderCreatedConsumer>(context);
            });
        });
    }
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

using Contracts;
using MassTransit;
using MassTransit.KafkaIntegration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);
var transport = builder.Configuration["TRANSPORT"]?.ToLowerInvariant() ?? "rabbitmq";
var rabbitHost = builder.Configuration["RABBITMQ_HOST"] ?? "rabbitmq";
var kafkaHost = builder.Configuration["KAFKA_HOST"] ?? "172-3-10-24.nip.io";
var kafkaPort = builder.Configuration["KAFKA_PORT"] ?? "9092";

builder.Services.AddMassTransit(cfg =>
{
    if (transport == "kafka")
    {
        // In-memory bus keeps minimal pipeline; Kafka rider handles produce.
        cfg.UsingInMemory((context, bus) => bus.ConfigureEndpoints(context));

        cfg.AddRider(rider =>
        {
            rider.AddProducer<OrderCreatedEvent>("order-created");
            rider.UsingKafka((context, k) =>
            {
                k.Host($"{kafkaHost}:{kafkaPort}");
            });
        });
    }
    else
    {
        cfg.UsingRabbitMq((context, bus) =>
        {
            bus.Host(rabbitHost, "/", h =>
            {
                h.Username("guest");
                h.Password("guest");
            });
        });
    }
});

var app = builder.Build();

app.MapPost("/api/orders", async (IPublishEndpoint bus, ITopicProducer<OrderCreatedEvent>? kafkaProducer, OrderRequest request) =>
{
    await Task.Delay(300); // DB mock
    var evt = new OrderCreatedEvent(Guid.NewGuid(), request.CustomerEmail, request.Total);

    if (kafkaProducer is not null)
    {
        await kafkaProducer.Produce(evt);
    }
    else
    {
        await bus.Publish(evt);
    }
    return Results.Accepted($"/api/orders/{evt.OrderId}", evt);
});

app.Run();

namespace Contracts
{
    public record OrderRequest(string CustomerEmail, decimal Total);
    public record OrderCreatedEvent(Guid OrderId, string CustomerEmail, decimal Total);
}

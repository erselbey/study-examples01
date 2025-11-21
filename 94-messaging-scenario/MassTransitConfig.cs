using MassTransit;
using System;
using System.Threading.Tasks;

public static class MassTransitConfig
{
    public static void Configure(IBusRegistrationConfigurator cfg)
    {
        cfg.AddConsumer<OrderCreatedConsumer>();
        cfg.AddConsumer<OrderPaidConsumer>();
        cfg.AddConsumer<OrderFailedConsumer>();
    }

    public static void ConfigureBus(IBusRegistrationContext context, IRabbitMqBusFactoryConfigurator bus)
    {
        bus.Host("rabbitmq", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        bus.Message<OrderCreatedEvent>(x => x.SetEntityName("orders-topic"));
        bus.Message<OrderPaidEvent>(x => x.SetEntityName("orders-topic"));
        bus.Message<OrderFailedEvent>(x => x.SetEntityName("orders-topic"));

        bus.ReceiveEndpoint("order-failed-queue", endpoint =>
        {
            endpoint.Bind("orders-topic", x => x.RoutingKey = "order.failed");
            endpoint.ConfigureConsumer<OrderFailedConsumer>(context);
            endpoint.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
            endpoint.UseDeadLetterQueue("order-failed-dlq");
        });
    }
}

class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
{
    public Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        Console.WriteLine($"Created: {context.Message.OrderId}");
        return Task.CompletedTask;
    }
}

class OrderPaidConsumer : IConsumer<OrderPaidEvent>
{
    public Task Consume(ConsumeContext<OrderPaidEvent> context)
    {
        Console.WriteLine($"Paid: {context.Message.OrderId}");
        return Task.CompletedTask;
    }
}

class OrderFailedConsumer : IConsumer<OrderFailedEvent>
{
    public Task Consume(ConsumeContext<OrderFailedEvent> context)
    {
        Console.WriteLine($"Failed: {context.Message.OrderId}");
        throw new InvalidOperationException("Simulated failure");
    }
}

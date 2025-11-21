# 94. Mesajlaşma Senaryosu – Retry & Dead-Letter

Amaç: OrderService'in farklı eventleri yayınladığı, NotificationService'in farklı kuyruklardan tüketip retry ve dead-letter mekanizmalarını yönettiği senaryo.

## Teknolojiler
- RabbitMQ topic exchange
- MassTransit

## Eventler & Routing Keys
- `OrderCreatedEvent` → routing key `order.created`
- `OrderPaidEvent` → `order.paid`
- `OrderFailedEvent` → `order.failed`

## Exchange & Kuyruk Yapısı
1. Topic exchange `orders-topic`.
2. Her event için ayrı queue (örn. `order-created-queue`).
3. Hatalı mesajlar için dead-letter queue (`order-failed-dlq`).

## MassTransit Örneği
```csharp
cfg.UsingRabbitMq((ctx, bus) =>
{
    bus.Host("rabbitmq");
    bus.Message<OrderCreatedEvent>(x => x.SetEntityName("orders-topic"));

    bus.ReceiveEndpoint("order-failed-queue", endpoint =>
    {
        endpoint.Bind("orders-topic", x => x.RoutingKey = "order.failed");
        endpoint.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
        endpoint.ConfigureConsumer<OrderFailedConsumer>(ctx);
        endpoint.UseDeadLetterQueue("order-failed-dlq");
    });
});
```

## Consumer Örnekleri
```csharp
public class OrderPaidConsumer : IConsumer<OrderPaidEvent>
{
    public Task Consume(ConsumeContext<OrderPaidEvent> context)
    {
        // ödeme maili
        return Task.CompletedTask;
    }
}

public class OrderFailedConsumer : IConsumer<OrderFailedEvent>
{
    public Task Consume(ConsumeContext<OrderFailedEvent> context)
    {
        throw new InvalidOperationException("Simulated failure");
    }
}
```
`OrderFailedConsumer` 3 kez retry sonrası DLQ'ya düşer.

## Notlar
- Dead-letter kuyruğunu RabbitMQ policy veya MassTransit `UseDelayedRedelivery` ile konfigüre et.
- Monitoring için RabbitMQ UI'dan queue metriklerini izle.

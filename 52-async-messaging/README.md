# 52. Async Mesajlaşma – Order & Notification Servisleri

Amaç: OrderService'in mesajı publish ettiği, NotificationService'in RabbitMQ üzerinden tüketip logladığı asenkron mimariyi göstermek.

## Teknolojiler
- .NET 9 Web API (OrderService & NotificationService)
- RabbitMQ + MassTransit
- Docker (lokal RabbitMQ) + docker-compose

## Hızlı Çalıştırma (sadece Docker ile)
`docker-compose.yml`, `OrderService.Dockerfile` ve `NotificationService.Dockerfile` hazır durumdadır. Lokal .NET kurulumu gerekmez.

```bash
cd 52-async-messaging
docker compose up --build
```

Servis portları:
- OrderService → `http://localhost:8081`
- NotificationService → `http://localhost:8082`
- RabbitMQ UI → `http://localhost:15672` (guest/guest)

## Mesaj Contract'ı
`Contracts/OrderCreatedEvent.cs`
```csharp
public record OrderCreatedEvent(Guid OrderId, string CustomerEmail, decimal Total);
```

## OrderService Minimal API
```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMassTransit(cfg =>
{
    cfg.UsingRabbitMq((ctx, bus) =>
    {
        bus.Host("rabbitmq", "/", h => { h.Username("guest"); h.Password("guest"); });
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
```

## NotificationService Consumer
```csharp
public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
{
    private readonly ILogger<OrderCreatedConsumer> _logger;
    public OrderCreatedConsumer(ILogger<OrderCreatedConsumer> logger) => _logger = logger;

    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        await Task.Delay(500); // async iş
        _logger.LogInformation("Order {OrderId} created → notification sent to {Email}",
            context.Message.OrderId, context.Message.CustomerEmail);
    }
}
```
MassTransit yapılandırması:
```csharp
builder.Services.AddMassTransit(cfg =>
{
    cfg.AddConsumer<OrderCreatedConsumer>();
    cfg.UsingRabbitMq((ctx, bus) =>
    {
        bus.Host("rabbitmq", "/", h => { h.Username("guest"); h.Password("guest"); });
        bus.ReceiveEndpoint("order-created-queue", endpoint =>
        {
            endpoint.ConfigureConsumer<OrderCreatedConsumer>(ctx);
        });
    });
});
```

## Test
```bash
curl -X POST http://localhost:8081/api/orders \
     -H "Content-Type: application/json" \
     -d '{"customerEmail":"kurs@example.com","total":25.5}'

docker compose logs -f notification-service
```

OrderService `202 Accepted` döner; NotificationService loglarında mesaj işlendiğini görürsün.

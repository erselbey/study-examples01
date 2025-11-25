# 64. Event-Driven Order, Inventory, Billing

Amaç: CQRS ile event-driven yaklaşımı birleştirip OrderService'in ürettiği eventleri Inventory ve Billing servislerinin tüketmesini sağlamak.

## Teknolojiler
- .NET 9
- RabbitMQ Event Bus (MassTransit veya custom bus)
- MediatR (OrderService command tarafı)

## Contracts Projesi
`OrderCreatedEvent`, `OrderCancelledEvent` gibi kayıtlar ortak projede tutulur.
```csharp
public record OrderCreatedEvent(Guid OrderId, string CustomerId, decimal Total);
```

## OrderService Akışı
1. `CreateOrderCommandHandler` siparişi EF Core ile DB'ye kaydeder.
2. Transaction sonrası `OrderCreatedEvent` event bus'a publish edilir.
3. `CancelOrderCommand` için `OrderCancelledEvent` basılır.

```csharp
public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
{
    private readonly OrdersDbContext _db;
    private readonly IEventBus _bus;
    public CreateOrderCommandHandler(OrdersDbContext db, IEventBus bus)
        => (_db, _bus) = (db, bus);

    public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken ct)
    {
        var order = new Order { /* ... */ };
        _db.Orders.Add(order);
        await _db.SaveChangesAsync(ct);
        await _bus.PublishAsync(new OrderCreatedEvent(order.Id, order.CustomerId, order.Total));
        return order.Id;
    }
}
```

## InventoryService Consumer
```csharp
public class InventoryOrderCreatedHandler : IIntegrationEventHandler<OrderCreatedEvent>
{
    public async Task Handle(OrderCreatedEvent @event)
    {
        // ilgili ürünlerin stoklarını düş
        await inventoryRepository.DecreaseStockAsync(@event.OrderId, @event.Total);
    }
}
```

## BillingService Consumer
Event'i dinleyip ödeme sürecini tetikler veya loglar:
```csharp
public class BillingOrderCreatedHandler : IIntegrationEventHandler<OrderCreatedEvent>
{
    private readonly ILogger<BillingOrderCreatedHandler> _logger;
    public BillingOrderCreatedHandler(ILogger<BillingOrderCreatedHandler> logger) => _logger = logger;

    public Task Handle(OrderCreatedEvent @event)
    {
        _logger.LogInformation("Payment started for order {OrderId}", @event.OrderId);
        return Task.CompletedTask;
    }
}
```

## Notlar
- Event bus olarak MassTransit, CAP veya custom RabbitMQ wrapper kullanılabilir.
- Idempotent tüketim için event log/outbox yaklaşımına hazırlıklı ol.

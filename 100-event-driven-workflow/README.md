# 100. Event-Driven Workflow (Saga Tarzı)

Amaç: Sipariş → ödeme → stok → bildirim akışını tamamen eventlerle yönetmek ve basit bir Orchestrator ile Saga durumunu tutmak.

## Servisler
- OrderService
- OrderOrchestratorService
- PaymentService
- InventoryService
- NotificationService

## Akış
1. `POST /api/orders` → OrderService `OrderCreatedEvent` yayınlar.
2. Orchestrator event'i dinleyip `PaymentRequestedEvent` yayınlar.
3. PaymentService sonucu `PaymentCompletedEvent` veya `PaymentFailedEvent` ile bildirir.
4. Başarılıysa Orchestrator `StockDecreaseRequestedEvent` yayınlar, InventoryService stok düşer.
5. Süreç tamamlandığında `OrderCompletedEvent` → NotificationService.

## Saga State (In-memory)
```csharp
public class OrderSagaState
{
    private readonly ConcurrentDictionary<Guid, SagaStatus> _orders = new();
    public SagaStatus Get(Guid orderId) => _orders.GetValueOrDefault(orderId);
    public void Update(Guid orderId, SagaStatus status) => _orders[orderId] = status;
}
```

## Orchestrator Consumer
```csharp
public class OrderCreatedHandler : IConsumer<OrderCreatedEvent>
{
    private readonly OrderSagaState _state;
    private readonly IPublishEndpoint _bus;

    public OrderCreatedHandler(OrderSagaState state, IPublishEndpoint bus) => (_state, _bus) = (state, bus);

    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        _state.Update(context.Message.OrderId, SagaStatus.AwaitingPayment);
        await _bus.Publish(new PaymentRequestedEvent(context.Message.OrderId, context.Message.Total));
    }
}
```

## Notlar
- State kalıcılığı için Redis veya SQL tablosu kullanılabilir.
- Zaman aşımı/tazminat senaryoları ekleyerek Saga'yı genişletebilirsin.

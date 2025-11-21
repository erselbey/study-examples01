using System;
using System.Collections.Concurrent;

public enum SagaStatus
{
    AwaitingPayment,
    PaymentFailed,
    PaymentCompleted,
    StockDecreased,
    Completed
}

public class OrderSagaState
{
    private readonly ConcurrentDictionary<Guid, SagaStatus> _orders = new();
    public SagaStatus Get(Guid orderId) => _orders.TryGetValue(orderId, out var status) ? status : SagaStatus.AwaitingPayment;
    public void Update(Guid orderId, SagaStatus status) => _orders[orderId] = status;
}

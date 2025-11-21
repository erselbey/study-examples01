using MassTransit;
using System.Threading.Tasks;

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

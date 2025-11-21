using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

public class PaymentRequestedHandler : IConsumer<PaymentRequestedEvent>
{
    private readonly ILogger<PaymentRequestedHandler> _logger;
    private readonly IPublishEndpoint _bus;

    public PaymentRequestedHandler(ILogger<PaymentRequestedHandler> logger, IPublishEndpoint bus)
        => (_logger, _bus) = (logger, bus);

    public async Task Consume(ConsumeContext<PaymentRequestedEvent> context)
    {
        _logger.LogInformation("Processing payment for order {OrderId}", context.Message.OrderId);
        await _bus.Publish(new PaymentCompletedEvent(context.Message.OrderId));
    }
}

public class PaymentCompletedHandler : IConsumer<PaymentCompletedEvent>
{
    private readonly IPublishEndpoint _bus;
    private readonly OrderSagaState _state;

    public PaymentCompletedHandler(IPublishEndpoint bus, OrderSagaState state) => (_bus, _state) = (bus, state);

    public async Task Consume(ConsumeContext<PaymentCompletedEvent> context)
    {
        _state.Update(context.Message.OrderId, SagaStatus.PaymentCompleted);
        await _bus.Publish(new StockDecreaseRequestedEvent(context.Message.OrderId));
    }
}

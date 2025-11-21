using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

public class StockDecreaseHandler : IConsumer<StockDecreaseRequestedEvent>
{
    private readonly ILogger<StockDecreaseHandler> _logger;
    private readonly IPublishEndpoint _bus;

    public StockDecreaseHandler(ILogger<StockDecreaseHandler> logger, IPublishEndpoint bus)
        => (_logger, _bus) = (logger, bus);

    public async Task Consume(ConsumeContext<StockDecreaseRequestedEvent> context)
    {
        _logger.LogInformation("Stock decreased for order {OrderId}", context.Message.OrderId);
        await _bus.Publish(new OrderCompletedEvent(context.Message.OrderId));
    }
}

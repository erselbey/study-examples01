using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<InventoryEventListener>();
    })
    .Build();

await host.RunAsync();

class InventoryEventListener : BackgroundService
{
    private readonly ILogger<InventoryEventListener> _logger;
    public InventoryEventListener(ILogger<InventoryEventListener> logger) => _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Demo amaçlı event bus entegrasyonu yerine fake loop
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(5000, stoppingToken);
            _logger.LogInformation("Inventory updated in response to OrderCreatedEvent");
        }
    }
}

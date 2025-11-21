using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
OrderCreatedEvent=$OrderCreatedBillingService
var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services => services.AddHostedService<BillingEventListener>())
    .Build();

await host.RunAsync();

class BillingEventListener : BackgroundService
{
    private readonly ILogger<BillingEventListener> _logger;
    public BillingEventListener(ILogger<BillingEventListener> logger) => _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(7000, stoppingToken);
            _logger.LogInformation("Billing workflow triggered for OrderCreatedEvent");
        }
    }
}

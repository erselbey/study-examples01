using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

public class OutboxPublisherHostedService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OutboxPublisherHostedService> _logger;

    public OutboxPublisherHostedService(IServiceScopeFactory scopeFactory, ILogger<OutboxPublisherHostedService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
            var pending = await db.OutboxMessages
                .Where(o => o.PublishedOn == null)
                .OrderBy(o => o.OccurredOn)
                .Take(20)
                .ToListAsync(stoppingToken);

            foreach (var message in pending)
            {
                _logger.LogInformation("Publishing {EventType} with payload {Payload}", message.EventType, message.Payload);
                message.PublishedOn = DateTime.UtcNow;
            }

            await db.SaveChangesAsync(stoppingToken);
            await Task.Delay(1000, stoppingToken);
        }
    }
}

public class OrdersDbContext : DbContext
{
    public OrdersDbContext(DbContextOptions<OrdersDbContext> options) : base(options) { }
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
}

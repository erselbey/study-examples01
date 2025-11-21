using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<OrderSagaState>();
        services.AddMassTransit(cfg =>
        {
            cfg.AddConsumer<OrderCreatedHandler>();
            cfg.AddConsumer<PaymentRequestedHandler>();
            cfg.AddConsumer<PaymentCompletedHandler>();
            cfg.AddConsumer<StockDecreaseHandler>();
            cfg.UsingInMemory((context, cfgBus) =>
            {
                cfgBus.ConfigureEndpoints(context);
            });
        });
    })
    .Build();

await host.RunAsync();

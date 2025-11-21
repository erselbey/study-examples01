# 119. Event-Driven Mikroservis Final Dokunuşları

Amaç: 118'deki sistemi production'a hazırlamak için outbox, saga state, circuit breaker, dağıtık tracing ve güvenlik kontrolleri eklemek.

## Outbox Pattern
```csharp
public class OutboxMessage
{
    public Guid Id { get; set; }
    public string EventType { get; set; } = default!;
    public string Payload { get; set; } = default!;
    public DateTime OccurredOn { get; set; }
    public DateTime? PublishedOn { get; set; }
}
```
- Command handler transaction içinde `OutboxMessages` tablosuna kaydeder.
- `OutboxPublisherHostedService` başarılı mesajları RabbitMQ'ya publish eder.

## Saga State DB
- `SagaStates` tablosu: `OrderId`, `Status`, `UpdatedAt`, `Metadata`.
- Orchestrator state'i kalıcı tutarak restart sonrası devam eder.

## Circuit Breaker + Retry (Polly)
```csharp
builder.Services.AddHttpClient("Payments")
    .AddTransientHttpErrorPolicy(p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)))
    .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(attempt * 2)));
```

## Distributed Tracing
```csharp
builder.Services.AddOpenTelemetry()
    .WithTracing(t =>
    {
        t.AddAspNetCoreInstrumentation()
         .AddHttpClientInstrumentation()
         .AddSource("CatalogService")
         .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("CatalogService"))
         .AddJaegerExporter();
    });
```

## Load Test & Security Review
- k6/JMeter ile temel load senaryosu çalıştır (`http.get("https://gateway.local/catalog/products")`).
- OWASP checklist: rate limiting, input validation, secrets management, TLS enforcement, dependency scanning.

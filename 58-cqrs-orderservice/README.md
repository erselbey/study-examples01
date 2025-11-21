# 58. CQRS OrderService

Amaç: Command (yazma) ve Query (okuma) yollarını MediatR ile ayrıştırmak.

## Teknolojiler
- .NET 8 Web API
- EF Core (SQL Server/PostgreSQL)
- MediatR

## Katmanlama
- **Domain/Infrastructure:** `OrdersDbContext`, `Order` entity.
- **Application:** Command & Query kayıtları ve handler'lar.
- **API:** Endpointler MediatR üzeriden handler'lara delegasyon yapar.

## Command Örneği
```csharp
public record CreateOrderCommand(string CustomerId, decimal Total) : IRequest<Guid>;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
{
    private readonly OrdersDbContext _db;
    public CreateOrderCommandHandler(OrdersDbContext db) => _db = db;

    public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = new Order { Id = Guid.NewGuid(), CustomerId = request.CustomerId, Total = request.Total };
        _db.Orders.Add(order);
        await _db.SaveChangesAsync(cancellationToken);
        return order.Id;
    }
}
```

## Query Örneği
```csharp
public record GetOrdersQuery : IRequest<IReadOnlyList<OrderDto>>;

public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, IReadOnlyList<OrderDto>>
{
    private readonly OrdersDbContext _db;
    public GetOrdersQueryHandler(OrdersDbContext db) => _db = db;

    public async Task<IReadOnlyList<OrderDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
        => await _db.Orders.AsNoTracking()
            .Select(o => new OrderDto(o.Id, o.CustomerId, o.Total))
            .ToListAsync(cancellationToken);
}

public record OrderDto(Guid Id, string CustomerId, decimal Total);
```

## API Endpointleri
```csharp
app.MapPost("/api/orders", async (IMediator mediator, CreateOrderCommand cmd) =>
{
    var id = await mediator.Send(cmd);
    return Results.Created($"/api/orders/{id}", new { id });
});

app.MapGet("/api/orders", (IMediator mediator) => mediator.Send(new GetOrdersQuery()));
```

## Notlar
- Command tarafında tam entity graph, query tarafında projection (DTO) kullanarak performansı artır.
- Unit test için handler'ları ayrı ayrı test etmek kolaydır.

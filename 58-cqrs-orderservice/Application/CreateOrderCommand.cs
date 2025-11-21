using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

public record CreateOrderCommand(string CustomerId, decimal Total) : IRequest<Guid>;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
{
    private readonly OrdersDbContext _db;
    public CreateOrderCommandHandler(OrdersDbContext db) => _db = db;

    public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = request.CustomerId,
            Total = request.Total
        };

        _db.Orders.Add(order);
        await _db.SaveChangesAsync(cancellationToken);
        return order.Id;
    }
}
//deneme 

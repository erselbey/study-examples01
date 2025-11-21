using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public record GetOrdersQuery : IRequest<IReadOnlyList<OrderDto>>;

public record OrderDto(Guid Id, string CustomerId, decimal Total);

public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, IReadOnlyList<OrderDto>>
{
    private readonly OrdersDbContext _db;
    public GetOrdersQueryHandler(OrdersDbContext db) => _db = db;

    public async Task<IReadOnlyList<OrderDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
        => await _db.Orders.AsNoTracking()
            .Select(o => new OrderDto(o.Id, o.CustomerId, o.Total))
            .ToListAsync(cancellationToken);
}

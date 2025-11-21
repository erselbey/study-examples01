using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<OrdersDbContext>(o => o.UseInMemoryDatabase("orders"));
builder.Services.AddSingleton<IEventBus, FakeEventBus>();

var app = builder.Build();

app.MapPost("/api/orders", async (OrdersDbContext db, IEventBus bus, CreateOrderRequest request) =>
{
    var order = new Order { Id = Guid.NewGuid(), CustomerId = request.CustomerId, Total = request.Total };
    db.Orders.Add(order);
    await db.SaveChangesAsync();

    await bus.PublishAsync(new OrderCreatedEvent(order.Id, order.CustomerId, order.Total));
    return Results.Created($"/api/orders/{order.Id}", order);
});

app.Run();

record CreateOrderRequest(string CustomerId, decimal Total);

class OrdersDbContext : DbContext
{
    public OrdersDbContext(DbContextOptions<OrdersDbContext> options) : base(options) { }
    public DbSet<Order> Orders => Set<Order>();
}

class Order
{
    public Guid Id { get; set; }
    public string CustomerId { get; set; } = default!;
    public decimal Total { get; set; }
}

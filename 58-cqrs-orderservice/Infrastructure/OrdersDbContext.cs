using Microsoft.EntityFrameworkCore;
using System;

public class OrdersDbContext : DbContext
{
    public OrdersDbContext(DbContextOptions<OrdersDbContext> options) : base(options)
    {
    }

    public DbSet<Order> Orders => Set<Order>();
}

public class Order
{
    public Guid Id { get; set; }
    public string CustomerId { get; set; } = default!;
    public decimal Total { get; set; }
}

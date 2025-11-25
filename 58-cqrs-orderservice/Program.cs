using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<OrdersDbContext>(options =>
    options.UseInMemoryDatabase("orders"));
builder.Services.AddMediatR(Assembly.GetExecutingAssembly());

var app = builder.Build();

app.MapPost("/api/orders", async (IMediator mediator, CreateOrderCommand command) =>
{
    var id = await mediator.Send(command);
    return Results.Created($"/api/orders/{id}", new { id });
});

app.MapGet("/api/orders", (IMediator mediator) => mediator.Send(new GetOrdersQuery()));

app.Run();


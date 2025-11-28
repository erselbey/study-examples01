using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;

var builder = WebApplication.CreateBuilder(args);

var serviceName = builder.Configuration["SERVICE_NAME"] ?? "catalog";
var environmentName = builder.Configuration["ENVIRONMENT"] ?? builder.Environment.EnvironmentName;

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.WriteIndented = true;
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

builder.Services.AddSingleton<IProductRepository, InMemoryProductRepository>();

var app = builder.Build();

app.MapGet("/", () => Results.Json(new
{
    status = "ok",
    service = serviceName,
    env = environmentName
}));

app.MapGet("/catalog/health", () => Results.Json(new
{
    status = "ok",
    service = serviceName,
    env = environmentName,
    timestamp = DateTimeOffset.UtcNow
}));

app.MapGet("/catalog/products", (IProductRepository repo) => Results.Ok(repo.GetAll()));

app.MapGet("/catalog/products/{id:int}", (IProductRepository repo, int id) =>
    repo.GetById(id) is { } product ? Results.Ok(product) : Results.NotFound($"Product {id} not found"));

app.MapPost("/catalog/products", (IProductRepository repo, Product product) =>
{
    repo.Add(product);
    return Results.Created($"/catalog/products/{product.Id}", product);
});

app.Run();

public record Product(int Id, string Name, decimal Price, int Stock);

public interface IProductRepository
{
    IEnumerable<Product> GetAll();
    Product? GetById(int id);
    void Add(Product product);
}

public sealed class InMemoryProductRepository : IProductRepository
{
    private readonly List<Product> _items = new()
    {
        new(1, "Mouse", 29.99m, 50),
        new(2, "Keyboard", 49.99m, 20)
    };

    public IEnumerable<Product> GetAll() => _items;
    public Product? GetById(int id) => _items.FirstOrDefault(p => p.Id == id);
    public void Add(Product product)
    {
        var nextId = _items.Any() ? _items.Max(p => p.Id) + 1 : 1;
        _items.Add(product with { Id = nextId });
    }
}

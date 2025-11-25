# 46. CatalogService – Basit Mikroservis

Amaç: Monolith'ten ayrılmış, tek başına çalışan minimal ürün kataloğu mikroservisini ayağa kaldırmak.

## Teknolojiler
- .NET 8 + ASP.NET Core Minimal API
- Swagger/OpenAPI
- In-memory repository (liste)

## Adımlar
1. `dotnet new webapi -n CatalogService` ile proje oluştur.
2. `Program.cs` içinde Minimal API endpointlerini yaz ve repository'yi DI ile ekle.
3. Swagger'i açarak endpointleri test et (`https://localhost:5001/swagger`).

## Örnek Kod
```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IProductRepository, InMemoryProductRepository>();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/api/products", (IProductRepository repo) => Results.Ok(repo.GetAll()));
app.MapGet("/api/products/{id:int}", (IProductRepository repo, int id) =>
    repo.GetById(id) is { } product ? Results.Ok(product) : Results.NotFound());
app.MapPost("/api/products", (IProductRepository repo, Product product) =>
{
    repo.Add(product);
    return Results.Created($"/api/products/{product.Id}", product);
});

app.Run();

record Product(int Id, string Name, decimal Price, int Stock);
interface IProductRepository
{
    IEnumerable<Product> GetAll();
    Product? GetById(int id);
    void Add(Product product);
}

sealed class InMemoryProductRepository : IProductRepository
{
    private readonly List<Product> _items = new()
    {
        new(1, "Mouse", 29.99m, 50),
        new(2, "Keyboard", 49.99m, 20)
    };

    public IEnumerable<Product> GetAll() => _items;
    public Product? GetById(int id) => _items.FirstOrDefault(p => p.Id == id);
    public void Add(Product product) => _items.Add(product with { Id = _items.Max(p => p.Id) + 1 });
}
```

## Çalıştırma
```bash
cd CatalogService
 dotnet run
```
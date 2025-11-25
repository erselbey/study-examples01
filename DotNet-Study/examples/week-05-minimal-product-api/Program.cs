using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// JSON seçeneklerini Türkçe kültürüne göre ayarlamak zorunda değilsin fakat örnek olarak gösteriyoruz.
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.WriteIndented = true;
});

builder.Services.AddSingleton<IProductRepository, InMemoryProductRepository>();

var app = builder.Build();

// Basit health endpoint'i: API'nin çalışıp çalışmadığını hızlıca kontrol etmek için kullanılır.
app.MapGet("/", () => Results.Json(new { status = "ok", service = "Product API" }));

app.MapGet("/products", (IProductRepository repo) =>
{
    // Repository soyutlaması sayesinde veri kaynağı değişse bile endpoint aynı kalır.
    return Results.Ok(repo.GetAll());
});

app.MapGet("/products/{id:int}", (int id, IProductRepository repo) =>
{
    var product = repo.GetById(id);
    return product is null ? Results.NotFound($"Ürün bulunamadı: {id}") : Results.Ok(product);
});

app.MapPost("/products", (ProductCreateRequest request, IProductRepository repo) =>
{
    if (string.IsNullOrWhiteSpace(request.Name))
    {
        return Results.BadRequest("Ürün adı boş bırakılamaz.");
    }

    var created = repo.Add(request);
    return Results.Created($"/products/{created.Id}", created);
});

app.MapDelete("/products/{id:int}", (int id, IProductRepository repo) =>
{
    var deleted = repo.Delete(id);
    return deleted ? Results.NoContent() : Results.NotFound($"Silinecek ürün bulunamadı: {id}");
});

app.Run();

// Basit ürün kayıt modeli
public record Product(int Id, string Name, decimal Price);

public record ProductCreateRequest(string Name, decimal Price);

public interface IProductRepository
{
    IReadOnlyCollection<Product> GetAll();
    Product? GetById(int id);
    Product Add(ProductCreateRequest request);
    bool Delete(int id);
}

public sealed class InMemoryProductRepository : IProductRepository
{
    private readonly List<Product> _products =
    [
        new Product(1, "Mouse", 300),
        new Product(2, "Klavye", 550)
    ];

    public IReadOnlyCollection<Product> GetAll() => _products.AsReadOnly();

    public Product? GetById(int id) => _products.Find(p => p.Id == id);

    public Product Add(ProductCreateRequest request)
    {
        var nextId = _products.Count == 0 ? 1 : _products[^1].Id + 1;
        var product = new Product(nextId, request.Name, request.Price);
        _products.Add(product);
        return product;
    }

    public bool Delete(int id)
    {
        var product = GetById(id);
        return product is not null && _products.Remove(product);
    }
}

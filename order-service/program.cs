using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;

// Web uygulamasının başlangıcını yapıyoruz
var builder = WebApplication.CreateBuilder(args);

// API için gerekli servisleri ekliyoruz
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Ürün ve sipariş reposunu singleton olarak ekliyoruz
builder.Services.AddSingleton<IProductRepository, InMemoryProductRepository>();
builder.Services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();

// Uygulamayı oluşturuyoruz
var app = builder.Build();

// Swagger arayüzünü aktif hale getiriyoruz
app.UseSwagger();
app.UseSwaggerUI();

// Ürünler API Uç Noktası: Tüm ürünleri listeleme
app.MapGet("/api/products", (IProductRepository repo) => Results.Ok(repo.GetAll()));

// Ürünler API Uç Noktası: Belirli bir ürünü ID ile getirme
app.MapGet("/api/products/{id:int}", (IProductRepository repo, int id) => 
    repo.GetById(id) is { } product ? Results.Ok(product) : Results.NotFound());

// Ürünler API Uç Noktası: Yeni ürün ekleme
app.MapPost("/api/products", (IProductRepository repo, Product product) =>
{
    repo.Add(product);
    return Results.Created($"/api/products/{product.Id}", product);
});

// Siparişler API Uç Noktası: Tüm siparişleri listeleme
app.MapGet("/api/orders", (IOrderRepository repo) => Results.Ok(repo.GetAll()));

// Siparişler API Uç Noktası: Belirli bir siparişi ID ile getirme
app.MapGet("/api/orders/{id:int}", (IOrderRepository repo, int id) => 
    repo.GetById(id) is { } order ? Results.Ok(order) : Results.NotFound());

// Siparişler API Uç Noktası: Yeni sipariş ekleme
app.MapPost("/api/orders", (IOrderRepository repo, Order order) =>
{
    repo.Add(order);
    return Results.Created($"/api/orders/{order.Id}", order);
});

// Uygulamayı başlatıyoruz
app.Run();

// Ürün kaydını temsil eden bir yapı
record Product(int Id, string Name, decimal Price, int Stock);

// Sipariş kaydını temsil eden bir yapı
record Order(int Id, int ProductId, int Quantity, decimal TotalPrice);

// Ürün reposu arayüzü
interface IProductRepository
{
    IEnumerable<Product> GetAll();      // Tüm ürünleri döndürme
    Product? GetById(int id);           // Belirli bir ürünü ID ile getirme
    void Add(Product product);           // Yeni ürün ekleme
}

// Sipariş reposu arayüzü
interface IOrderRepository
{
    IEnumerable<Order> GetAll();        // Tüm siparişleri döndürme
    Order? GetById(int id);             // Belirli bir siparişi ID ile getirme
    void Add(Order order);               // Yeni sipariş ekleme
}

// Bellekte ürünleri saklayan sınıf
sealed class InMemoryProductRepository : IProductRepository
{
    private readonly List<Product> _items = new()
    {
        new(1, "Mouse", 29.99m, 50),     // Örnek ürünler
        new(2, "Keyboard", 49.99m, 20)
    };

    // Tüm ürünleri döndüren metod
    public IEnumerable<Product> GetAll() => _items;

    // Belirli bir ürünü ID ile getiren metod
    public Product? GetById(int id) => _items.FirstOrDefault(p => p.Id == id);

    // Yeni ürün ekleyen metod
    public void Add(Product product)
    {
        var nextId = _items.Any() ? _items.Max(p => p.Id) + 1 : 1; // ID'yi otomatik artır
        _items.Add(product with { Id = nextId }); // Yeni ürünü listeye ekle
    }
}

// Bellekte siparişleri saklayan sınıf
sealed class InMemoryOrderRepository : IOrderRepository
{
    private readonly List<Order> _orders = new();

    // Tüm siparişleri döndüren metod
    public IEnumerable<Order> GetAll() => _orders;

    // Belirli bir siparişi ID ile getiren metod
    public Order? GetById(int id) => _orders.FirstOrDefault(o => o.Id == id);

    // Yeni sipariş ekleyen metod
    public void Add(Order order)
    {
        var nextId = _orders.Any() ? _orders.Max(o => o.Id) + 1 : 1; // ID'yi otomatik artır
        _orders.Add(order with { Id = nextId, TotalPrice = CalculateTotalPrice(order) }); // Yeni siparişi ekle
    }

    // Siparişin toplam fiyatını hesaplayan metod
    private decimal CalculateTotalPrice(Order order)
    {
        var productRepo = new InMemoryProductRepository(); // Ürün reposunu oluştur
        var product = productRepo.GetById(order.ProductId); // Ürünü bul
        return product?.Price * order.Quantity ?? 0; // Toplam fiyatı hesapla
    }
}

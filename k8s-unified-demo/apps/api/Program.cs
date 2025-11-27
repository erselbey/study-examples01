using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;

var builder = WebApplication.CreateBuilder(args);

var serviceName = builder.Configuration["SERVICE_NAME"] ?? "api";
var environmentName = builder.Configuration["ENVIRONMENT"] ?? builder.Environment.EnvironmentName;
var workerUrl = builder.Configuration["WORKER_URL"] ?? "http://worker:8081";

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.WriteIndented = true;
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

builder.Services.AddSingleton<IProductRepository, InMemoryProductRepository>();
builder.Services.AddHttpClient("worker", client =>
{
    client.BaseAddress = new Uri(workerUrl);
    client.Timeout = TimeSpan.FromSeconds(3);
});

var app = builder.Build();

app.MapGet("/", () => Results.Json(new
{
    status = "ok",
    service = serviceName,
    env = environmentName,
    worker = workerUrl
}));

app.MapGet("/health", () => Results.Json(new
{
    status = "ok",
    service = serviceName,
    env = environmentName
}));

app.MapGet("/products", (IProductRepository repo) => Results.Ok(repo.GetAll()));

app.MapGet("/products/{id:int}", (int id, IProductRepository repo) =>
{
    var product = repo.GetById(id);
    return product is null ? Results.NotFound($"Product {id} not found") : Results.Ok(product);
});

app.MapPost("/products", (ProductCreateRequest request, IProductRepository repo) =>
{
    if (string.IsNullOrWhiteSpace(request.Name))
    {
        return Results.BadRequest("Product name is required");
    }

    var created = repo.Add(request);
    return Results.Created($"/products/{created.Id}", created);
});

app.MapDelete("/products/{id:int}", (int id, IProductRepository repo) =>
{
    var removed = repo.Delete(id);
    return removed ? Results.NoContent() : Results.NotFound($"Product {id} not found");
});

app.MapPost("/call-worker", async (WorkerRequest payload, IHttpClientFactory factory) =>
{
    var client = factory.CreateClient("worker");
    try
    {
        var response = await client.PostAsJsonAsync("/tasks", payload);
        if (!response.IsSuccessStatusCode)
        {
            return Results.Problem($"Worker returned {response.StatusCode}");
        }

        var result = await response.Content.ReadFromJsonAsync<object>();
        return Results.Ok(new { source = serviceName, workerResponse = result });
    }
    catch (Exception ex)
    {
        return Results.Json(new { source = serviceName, error = ex.Message }, statusCode: 502);
    }
});

app.Run();

public record Product(int Id, string Name, decimal Price);

public record ProductCreateRequest(string Name, decimal Price);

public record WorkerRequest(string Job, string? Note);

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
        new Product(2, "Keyboard", 550)
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

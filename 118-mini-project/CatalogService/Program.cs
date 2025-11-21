using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.MapGet("/api/catalog/products", () => Results.Ok(new[] { new { Id = 1, Name = "Mouse" } }));
app.Run();

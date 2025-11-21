using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/api/catalog", () => Results.Ok(new[]
{
    new { Id = 1, Name = "Mouse", Price = 29.99m },
    new { Id = 2, Name = "Keyboard", Price = 49.99m }
}));

app.Run();
// End of file
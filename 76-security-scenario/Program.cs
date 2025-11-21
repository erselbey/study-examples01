using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SuperSecretKey123!"));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = "security-demo",
            ValidAudience = "security-clients",
            IssuerSigningKey = key,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CanDeleteProduct", policy =>
        policy.RequireRole("Admin").RequireClaim("scope", "catalog.delete"));
});

var app = builder.Build();
app.UseHttpsRedirection();
app.UseHsts();
app.Use(async (ctx, next) =>
{
    ctx.Response.Headers["X-Content-Type-Options"] = "nosniff";
    ctx.Response.Headers["X-Frame-Options"] = "DENY";
    await next();
});
app.UseCors(policy =>
    policy.WithOrigins("https://trusted.app")
          .AllowAnyHeader()
          .AllowAnyMethod());
app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/api/products", [Authorize(Roles = "Admin")] (Product product) => Results.Ok(product));
app.MapDelete("/api/products/{id}", [Authorize(Policy = "CanDeleteProduct")] (int id) => Results.NoContent());

app.MapGet("/api/orders/my", [Authorize(Roles = "User")] (ClaimsPrincipal user) =>
{
    var userId = user.FindFirstValue("sub");
    var orders = new[]
    {
        new { Id = 1, CustomerId = userId, Total = 19.99m },
        new { Id = 2, CustomerId = "another", Total = 29.99m }
    };

    return Results.Ok(orders.Where(o => o.CustomerId == userId));
});

app.Run();

record Product(string Name, decimal Price);

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SuperSecretKey123!"));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = "demo-auth",
            ValidAudience = "demo-clients",
            IssuerSigningKey = key,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true
        };
    });

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/api/auth/login", (LoginRequest request) =>
{
    if (request.Username != "demo" || request.Password != "Pass@123")
    {
        return Results.Unauthorized();
    }

    var token = JwtTokenGenerator.GenerateToken(request.Username, key);
    return Results.Ok(new { access_token = token });
});

app.MapGet("/api/orders", [Authorize] () => Results.Ok(new[] { new { Id = 1, Total = 42.5m } }));
app.MapPost("/api/orders", [Authorize] (OrderRequest order) => Results.Ok(order));

app.Run();

record LoginRequest(string Username, string Password);
record OrderRequest(decimal Total);

static class JwtTokenGenerator
{
    public static string GenerateToken(string username, SymmetricSecurityKey key)
    {
        var descriptor = new Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor
        {
            Issuer = "demo-auth",
            Audience = "demo-clients",
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, "User")
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        };

        var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var token = handler.CreateToken(descriptor);
        return handler.WriteToken(token);
    }
}

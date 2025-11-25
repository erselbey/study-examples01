# 70. JWT ile Korunan OrderService

Amaç: AuthService üzerinden alınan JWT token olmadan OrderService endpointlerine erişilememesi.

## Teknolojiler
- .NET 9 Web API
- JWT Bearer Authentication
- Basit AuthService (hardcoded user veya Identity)

## AuthService – Login Endpoint
```csharp
app.MapPost("/api/auth/login", (LoginRequest request) =>
{
    if (request.Username != "demo" || request.Password != "Pass@123")
        return Results.Unauthorized();

    var token = JwtTokenGenerator.Generate(request.Username);
    return Results.Ok(new { access_token = token });
});

record LoginRequest(string Username, string Password);
```

## Token Üretimi
```csharp
public static class JwtTokenGenerator
{
    public static TokenValidationParameters Parameters => new()
    {
        ValidIssuer = "demo-auth",
        ValidAudience = "demo-clients",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SuperSecretKey123!")),
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true
    };

    public static string Generate(string username)
    {
        var handler = new JwtSecurityTokenHandler();
        var token = handler.CreateToken(new SecurityTokenDescriptor
        {
            Issuer = Parameters.ValidIssuer,
            Audience = Parameters.ValidAudience,
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, "User")
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(
                (SymmetricSecurityKey)Parameters.IssuerSigningKey!, SecurityAlgorithms.HmacSha256)
        });
        return handler.WriteToken(token);
    }
}
```

## OrderService – JWT Konfigürasyonu
```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = JwtTokenGenerator.Parameters;
    });

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/api/orders", [Authorize] () => Results.Ok(/* veri */));
app.MapPost("/api/orders", [Authorize] (Order order) => Results.Ok(order));
```

## Postman Akışı
1. AuthService'e `POST /api/auth/login` → token al.
2. OrderService çağrılarına `Authorization: Bearer <token>` header'ı ekle.

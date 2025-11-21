# 76. Role/Claim Bazlı Güvenlik Senaryosu

Amaç: JWT içindeki role/claim bilgilerini kullanarak servisleri korumak ve temel OWASP önlemlerini uygulamak.

## Teknolojiler
- AuthService + CatalogService + OrderService
- Role-based & claims-based authorization
- ASP.NET Core güvenlik middleware'leri

## JWT Claim'leri
- `sub`: kullanıcı Id'si
- `role`: `Admin` veya `User`
- Ek claim: `permissions` vb.

## CatalogService Yetkilendirme
```csharp
[Authorize(Roles = "Admin")]
[HttpPost("api/products")]
public IActionResult CreateProduct(Product product) => Ok(product);

[Authorize(Roles = "Admin")]
[HttpDelete("api/products/{id}")]
public IActionResult DeleteProduct(int id) => NoContent();
```

## OrderService – Kullanıcıya Ait Siparişler
```csharp
[Authorize(Roles = "User")]
[HttpGet("api/orders/my")]
public IActionResult GetMyOrders()
{
    var userId = User.FindFirstValue("sub");
    var orders = _db.Orders.Where(o => o.CustomerId == userId);
    return Ok(orders);
}
```

## OWASP Dostu Middleware Örneği
```csharp
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
```

## Notlar
- Role + claim kombinasyonu ile daha ince yetkilendirme yapılabilir (`[Authorize(Policy = "CanDeleteProduct")]`).
- Rate limiting, input validation ve secret management gibi ek maddeler checklist'te yer almalı.

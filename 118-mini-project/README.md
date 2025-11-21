# 118. Mini Proje – Full Mikroservis Uygulaması

Amaç: Catalog, Basket, Order, Payment, Inventory, Notification, Auth ve ApiGateway servislerini kapsayan mikroservis ekosistemi kurmak.

## Bileşenler
- **Servisler:** her biri ayrı .NET projesi, kendi DB'si (PostgreSQL/SQL Server).
- **Event Bus:** RabbitMQ + MassTransit (event-driven sipariş süreci).
- **Auth:** JWT + role-based authorization.
- **API Gateway:** Ocelot (tek giriş noktası).
- **Observability:** Prometheus/Grafana, merkezi log (Seq/ELK), OpenTelemetry.

## Ocelot Örneği
`ocelot.json`
```json
{
  "Routes": [
    {
      "DownstreamPathTemplate": "/api/catalog/{everything}",
      "UpstreamPathTemplate": "/catalog/{everything}",
      "DownstreamHostAndPorts": [{ "Host": "catalog-service", "Port": 8080 }]
    }
  ],
  "GlobalConfiguration": { "BaseUrl": "https://gateway.local" }
}
```

## Docker Compose İskeleti
```yaml
services:
  catalog:
    build: ./CatalogService
    ports: ["8080:8080"]
  rabbitmq:
    image: rabbitmq:3-management
    ports: ["5672:5672", "15672:15672"]
  gateway:
    build: ./ApiGateway
    ports: ["9000:8080"]
    depends_on: [catalog, basket, order]
```

## İpuçları
- Her servis bağımsız olarak deploy edilebilir olmalı (CI/CD pipeline'ları ayrı).
- Event schemaları için Contracts projesi oluştur, paket olarak paylaş.
- OpenTelemetry tracer ile servisler arası traceId yayılımını sağla (`AddAspNetCoreInstrumentation`).

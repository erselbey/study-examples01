## Mikroservis Mini Uygulamaları Arşivi

Notion'daki 14 egzersizin her biri artık kendi klasörü ve README'si ile ayrıştırıldı. Aşağıdaki tablo hangi klasörde hangi mini uygulamanın bulunduğunu ve hızlıca neler içerdiğini gösterir.

| #  | Klasör | İçerik Özeti |
| --- | --- | --- |
| 46 | [46-catalog-service](46-catalog-service) | Minimal API tabanlı CatalogService; `Program.cs` içinde in-memory repo + Swagger örneği. |
| 52 | [52-async-messaging](52-async-messaging) | Order & Notification servisleri için iki ayrı `Program.cs`, RabbitMQ + MassTransit publish/consume kodu. |
| 58 | [58-cqrs-orderservice](58-cqrs-orderservice) | MediatR command/query handler'ları, EF Core `OrdersDbContext` ve Minimal API giriş noktası. |
| 64 | [64-event-driven-microservice](64-event-driven-microservice) | Order, Inventory, Billing servis örnekleri + Fake event bus ve Contracts klasörü. |
| 70 | [70-secure-orderservice](70-secure-orderservice) | Auth & Order endpointlerini içeren JWT korumalı Minimal API. |
| 76 | [76-security-scenario](76-security-scenario) | Role/claim bazlı yetkilendirme, güvenlik header'ları ve örnek ürün/sipariş endpointleri. |
| 82 | [82-dockerized-catalogservice](82-dockerized-catalogservice) | CatalogService Minimal API kodu + Dockerfile ile container'a hazır yapı. |
| 88 | [88-k8s-deploy](88-k8s-deploy) | Gerçek `k8s/deployment.yaml` ve `service.yaml` manifest dosyaları. |
| 94 | [94-messaging-scenario](94-messaging-scenario) | Order eventleri, MassTransit consumer/config kodları ve health endpointi. |
| 100 | [100-event-driven-workflow](100-event-driven-workflow) | Saga state sınıfı, handler'lar ve MassTransit in-memory host ile orchestrator örneği. |
| 106 | [106-cicd-workflow](106-cicd-workflow) | `.github/workflows/cicd.yaml` pipeline dosyası. |
| 112 | [112-end-to-end-deploy](112-end-to-end-deploy) | Registry push sonrası kubectl rollout yapan pipeline dosyası. |
| 118 | [118-mini-project](118-mini-project) | Docker Compose, Ocelot konfigürasyonu ve örnek CatalogService kodu. |
| 119 | [119-event-driven-final](119-event-driven-final) | Outbox entity, hosted service, Polly + OpenTelemetry entegrasyonlu Program.cs. |

> Her klasördeki README, ilgili senaryonun amaçlarını, teknoloji yığınını, örnek kod parçacıklarını ve çalıştırma adımlarını içerir. Sıralamayı takip ederek monolith'ten production-ready mikroservis mimarisine doğru ilerleyebilirsiniz.
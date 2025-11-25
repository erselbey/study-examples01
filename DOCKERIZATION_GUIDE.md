# Mikroservis Dockerizasyonu ve Uçtan Uca Ödev Dokümantasyonu

Bu doküman, `/Users/erselbey/Downloads/study-examples01` klasöründeki projeleri sıfırdan kurup Docker konteynerlerinde çalıştırmak isteyen kişiler için hazırlandı. Her bölümde:

- **Amaç & teknoloji özeti**
- **Dockerize etme adımları ve gerekli komut satırları**
- **Servisin içinde/yakınında çalıştırılacak test komutları ve ileri düzey ödev önerileri**

Projeler basitten karmaşığa doğru sıralandı. Tüm örnekler `.NET 9` tabanlıdır.

## 0. Başlarken

```bash
# Gerekli araçlar (macOS/Homebrew örneği)
brew install --cask docker
brew install dotnet
brew install kubectl
brew install k3d
brew install helm

# Repo köküne gel
cd /Users/erselbey/Downloads/study-examples01
```

Docker Desktop arayüzünü açık tut ve `docker --version`, `dotnet --info` komutlarının çıktısını doğrula.

> **Not:** Klasörlerin çoğunda yalnızca `Program.cs` ve README bulunuyor. Kursiyerler, ilgili klasöre girip `dotnet new webapi -n <ProjeAdi>` (veya `dotnet new worker`) komutlarıyla .csproj iskeletini oluşturmalı, ardından verilen `Program.cs` içeriğini bu repo ile senkronlamalıdır.

Çoğu servis için aşağıdaki çok aşamalı Dockerfile şablonunu kullanabilirsiniz:

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "<ProjectName>.dll"]
```

`<ProjectName>` kısmını kendi `.csproj` adınla değiştirmen yeterli.

---

## 1. `46-catalog-service` – Minimal API’yi Dockerize Et

- **Hedef:** Minimal .NET API’yi tek başına imaj haline getirmek ve Swagger’dan test etmek.
- **Kaynak dosyalar:** `46-catalog-service/Program.cs`

```bash
cd 46-catalog-service
dotnet restore
dotnet publish -c Release -o out
```

`46-catalog-service` klasörüne şu içeriğiyle bir `Dockerfile` ekleyin:

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CatalogService.dll"]
```

> DLL adını `dotnet new webapi -n <ProjeAdi>` sırasında belirlediğiniz proje adıyla eşleştirin.

İmajı oluşturup ayağa kaldır:

```bash
docker build -t demo/catalog-service:1.0 .
docker run --rm -p 8080:8080 --name catalog demo/catalog-service:1.0
curl http://localhost:8080/swagger/index.html
```

**Ödev:** Yeni bir `PUT /api/products/{id}` endpoint’i ekleyip, imajı `demo/catalog-service:1.1` etiketiyle tekrar yayınla.

---

## 2. `52-async-messaging` – Order & Notification Servislerini Konteynerize Et

- **Hedef:** RabbitMQ’ya bağlı iki mikroservisi docker-compose ile ayağa kaldırmak.
- **Kaynak:** `52-async-messaging/OrderService.Program.cs`, `NotificationService.Program.cs`

Önce Docker ağı için RabbitMQ hazırla:

```bash
docker network create orders-net
docker run -d --name rabbitmq --network orders-net -p 5672:5672 -p 15672:15672 rabbitmq:3-management
```

Her servis için `Dockerfile` (aynı içerik, dosyayı ilgili klasörlere kopyala):

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "OrderService.dll"]  # NotificationService için dll adını değiştir
```

İmajları oluşturup çalıştır:

```bash
cd 52-async-messaging/OrderService
docker build -t demo/order-service .
docker run -d --name order-service --network orders-net -p 8081:8080 demo/order-service

cd ../NotificationService
docker build -t demo/notification-service .
docker run -d --name notification-service --network orders-net -p 8082:8080 demo/notification-service
```

Test çağrısı:

```bash
curl -X POST http://localhost:8081/api/orders \
     -H "Content-Type: application/json" \
     -d '{"customerEmail":"kurs@example.com","total":25.5}'
docker logs notification-service
```

**Ödev:** Yukarıdaki adımları `docker-compose.yml` içine taşı ve yalnızca `docker compose up` komutuyla tüm sistemi başlat.

---

## 3. `58-cqrs-orderservice` – Command/Query Servisini Container Üzerinden Çalıştır

- **Hedef:** MediatR tabanlı API’yi container içinde yayınlamak.
- **Kaynak:** `58-cqrs-orderservice/*`

```bash
cd 58-cqrs-orderservice
dotnet restore
dotnet test || true   # Test yoksa hata beklenmez
```

Multi-stage Dockerfile:

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CqrsOrderService.dll"]  # Proje adını kendi csproj'unuza göre güncelleyin
```

İmaj ve test:

```bash
docker build -t demo/cqrs-order-service .
docker run --rm -p 8083:8080 demo/cqrs-order-service
curl -X POST http://localhost:8083/api/orders -H "Content-Type: application/json" \
     -d '{"customerId":"demo","total":99.5}'
curl http://localhost:8083/api/orders
```

**Ödev:** Container’a `ASPNETCORE_ENVIRONMENT=Staging` değişkeni vererek farklı bir EF Core sağlayıcısı kullanmayı deneyin (ör. PostgreSQL imajı ekleyin).

---

## 4. `64-event-driven-microservice` – Order/Inventory/Billing’i Çoklu Servis Olarak Çalıştır

- **Hedef:** OrderService’i HTTP API, Inventory/Billing’i arka plan worker olarak Docker ortamına taşımak.
- **Kaynak:** `64-event-driven-microservice/*`

Önerilen `docker-compose.yml` taslağı:

```yaml
version: "3.9"
services:
  order:
    build:
      context: ./64-event-driven-microservice
      dockerfile: OrderService.Dockerfile
    ports:
      - "8084:8080"
  inventory:
    build:
      context: ./64-event-driven-microservice
      dockerfile: InventoryService.Dockerfile
  billing:
    build:
      context: ./64-event-driven-microservice
      dockerfile: BillingService.Dockerfile
```

Her Dockerfile, ilgili `Program.cs` dosyasını `dotnet publish` edip doğru DLL ile başlatmalı.

Çalıştırma:

```bash
cd 64-event-driven-microservice
docker compose up --build
curl -X POST http://localhost:8084/api/orders -H "Content-Type: application/json" \
     -d '{"customerId":"C1","total":150}'
docker compose logs inventory | tail
docker compose logs billing | tail
```

**Ödev:** Fake event bus yerine RabbitMQ’yu compose dosyasına ekleyip gerçek publish/consume zinciri kur.

---

## 5. `70-secure-orderservice` – JWT Korumalı Servis

- **Hedef:** Auth + Order endpoint’lerini aynı konteynerde çalıştırıp token ile test etmek.
- **Kaynak:** `70-secure-orderservice/Program.cs`

```bash
cd 70-secure-orderservice
dotnet publish -c Release -o out
docker build -t demo/secure-order-service .
docker run --rm -p 8085:8080 demo/secure-order-service
```

Token al ve kullan:

```bash
ACCESS_TOKEN=$(curl -s -X POST http://localhost:8085/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"demo","password":"Pass@123"}' | jq -r '.access_token')

curl http://localhost:8085/api/orders -H "Authorization: Bearer $ACCESS_TOKEN"
```

**Ödev:** `docker run` sırasında `-e Jwt__Issuer=kurs-demo` gibi environment değişkenleriyle token parametrelerini dışsallaştır.

---

## 6. `76-security-scenario` – Role/Claim Tabanlı Güvenlik

- **Hedef:** Policy, CORS ve güvenlik header’larıyla sertleştirilmiş servisi konteynerde deneyimlemek.
- **Kaynak:** `76-security-scenario/Program.cs`

Komutlar:

```bash
cd 76-security-scenario
dotnet publish -c Release -o out
docker build -t demo/security-scenario .
docker run --rm -p 8086:8080 demo/security-scenario
```

Swagger olmadan test için JWT üret (70. projeden kopya token olabilir) ve farklı roller içeren payload’lar gönder.

**Ödev:** `Program.cs` içine `app.UseRateLimiter` ekleyip, container yeniden inşa ederek saldırı senaryosu testleri çalıştır.

---

## 7. `82-dockerized-catalogservice` – Hazır Dockerfile ile Hızlı Dağıtım

- **Hedef:** Var olan Dockerfile’ı kullanarak CI’ye uygun imaj üretmek.
- **Kaynak:** `82-dockerized-catalogservice/Dockerfile`, `Program.cs`

```bash
cd 82-dockerized-catalogservice
docker build -t demo/catalog-multistage:latest .
docker run --rm -p 8087:8080 demo/catalog-multistage:latest
curl http://localhost:8087/api/catalog
```

**Ödev:** Dockerfile’a `HEALTHCHECK --interval=30s CMD curl -f http://localhost:8080/health || exit 1` satırı ekle ve imajı tekrar oluştur.

---

## 8. `88-k8s-deploy` – Kubernetes’e Dağıt

- **Hedef:** Docker imajı registry’e pushlandıktan sonra manifestlerle K8s’e almak.
- **Kaynak:** `88-k8s-deploy/k8s/*.yaml`

```bash
cd 88-k8s-deploy
# Docker imajını önceden registry’e (ör. ghcr.io/<kullanıcı>/catalog) pushla
kubectl apply -f k8s/deployment.yaml
kubectl apply -f k8s/service.yaml
kubectl get pods
kubectl port-forward svc/catalog-service 5000:80
curl http://localhost:5000/swagger/index.html
```

**Ödev:** Deployment’a `livenessProbe` ve `readinessProbe` ekleyip rollout’u tekrar çalıştır.

---

## 9. `94-messaging-scenario` – Retry & Dead-Letter Demo

- **Hedef:** MassTransit konfigürasyonunu container’da çalıştırıp RabbitMQ kuyruklarını gözlemlemek.
- **Kaynak:** `94-messaging-scenario/*`

```bash
docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management
cd 94-messaging-scenario
docker build -t demo/messaging-scenario .
docker run --rm --network host demo/messaging-scenario
```

Mesaj tetikle (örnek script):

```bash
dotnet script send-orders.csx  # MassTransit IBus ile publish eden küçük bir script yazın
```

RabbitMQ UI’dan `order-failed-dlq` kuyruğuna düşen mesajları incele.

**Ödev:** Container’a `OrderPaidEvent` publish eden bir CLI ekleyip `docker compose` ile birlikte çalıştır.

---

## 10. `100-event-driven-workflow` – Saga/Orchestrator’ı İzole Et

- **Hedef:** In-memory MassTransit ile saga state’i yöneten orkestratörü dockerize etmek.
- **Kaynak:** `100-event-driven-workflow/*`

```bash
cd 100-event-driven-workflow
docker build -t demo/order-saga .
docker run --rm -p 8088:8080 demo/order-saga   # HTTP endpoint yoksa port atlaması opsiyonel
```

Test için MassTransit’in `IBus`’ını kullanan küçük bir publisher yazın veya `dotnet script` ile `PaymentRequestedEvent` yayın.

**Ödev:** Container içinde `dotnet watch` ile hot reload çalıştırmayı deneyip event akışına yeni adımlar (örn. `OrderCancelledEvent`) ekleyin.

---

## 11. `106-cicd-workflow` – CI Pipeline’da Docker Build

- **Hedef:** GitHub Actions üzerinden imaj oluşturup pushlamak.
- **Kaynak:** `106-cicd-workflow/README.md`

Lokal doğrulama:

```bash
cd 106-cicd-workflow
gh workflow run cicd.yaml --repo <org>/study-examples --ref main  # GitHub CLI ile manuel tetikle
```

Docker build aşamasını yerelde taklit etmek için:

```bash
docker build -t ghcr.io/<org>/catalog-service:local-test ../82-dockerized-catalogservice
docker run --rm ghcr.io/<org>/catalog-service:local-test dotnet --info
```

**Ödev:** Workflow’a `dotnet format --verify-no-changes` adımı ekleyip pipeline’ı yeniden tetikle.

---

## 12. `112-end-to-end-deploy` – CI + Kubernetes

- **Hedef:** Aynı pipeline içinde `kubectl` ile rollout yapmak.

Lokalde kubectl set image akışını prova et:

```bash
kubectl config use-context <hedef-cluster>
kubectl set image deployment/catalog-deployment \
    catalog=ghcr.io/<org>/catalog-service:test-sha
kubectl rollout status deployment/catalog-deployment
kubectl run curl --rm -i --restart=Never --image=curlimages/curl \
    -- curl http://catalog-service/health
```

**Ödev:** Pipeline’a `kubectl apply -f 88-k8s-deploy/k8s` step’i ekleyip `KUBE_CONFIG` secret’ıyla çalıştır.

---

## 13. `118-mini-project` – Çok Servisli Compose

- **Hedef:** Gateway, Catalog ve diğer servisleri tek docker-compose ile ayağa kaldırmak.
- **Kaynak:** `118-mini-project/docker-compose.yml`, `ApiGateway/ocelot.json`, `CatalogService/Program.cs`

```bash
cd 118-mini-project
docker compose up --build
curl http://localhost:8080/api/catalog    # catalog servisi
curl http://localhost:9000/catalog/api/catalog -H "Ocp-Apim-Subscription-Key: demo"  # gateway üzerinden örnek
```

Eksik servisler (Basket, Order vb.) için klasörler açıp `dotnet new webapi -n BasketService` komutuyla projeleri ekleyin ve compose dosyasındaki `build` yollarını güncelleyin.

**Ödev:** RabbitMQ, PostgreSQL ve Prometheus’u compose’a ekleyip her servisten environment değişkenleriyle bağlantı kurun.

---

## 14. `119-event-driven-final` – Production Sertleşmesi

- **Hedef:** Outbox, circuit breaker ve OpenTelemetry’yi içeren son sürümü dockerize etmek.
- **Kaynak:** `119-event-driven-final/*`

```bash
cd 119-event-driven-final
docker build -t demo/order-service-prod .
docker run --rm -p 8089:8080 -e OTEL_EXPORTER_OTLP_ENDPOINT=http://otel-collector:4317 demo/order-service-prod
curl http://localhost:8089/health
docker logs <container_id> | grep Outbox
```

Outbox tablosunu persist etmek için container’a volume bağlayın veya gerçek bir PostgreSQL kullanın.

**Ödev:** `OutboxPublisherHostedService` loglarını Loki’ye yönlendirmek için Serilog + Grafana Loki sink’i ekleyin ve yeniden paketleyin.

---

## 15. `monitoring-logging-onprem` – Observability Stack’i Ayağa Kaldır

- **Hedef:** Prometheus + Grafana + Loki + Promtail’i k3d üzerinde kurmak.
- **Kaynak:** `monitoring-logging-onprem/k3d-config.yaml`, `manifests/*`

```bash
cd monitoring-logging-onprem
k3d cluster create --config k3d-config.yaml
export KUBECONFIG=$(k3d kubeconfig write observability)
kubectl apply -f manifests/namespace.yaml
kubectl apply -f manifests/prometheus-configmap.yaml -f manifests/prometheus-deployment.yaml
kubectl apply -f manifests/grafana-configmap.yaml -f manifests/grafana-deployment.yaml
kubectl apply -f manifests/loki-configmap.yaml -f manifests/loki-statefulset.yaml
kubectl apply -f manifests/promtail-configmap.yaml -f manifests/promtail-daemonset.yaml
```

Port-forward:

```bash
kubectl port-forward -n observability svc/prometheus 9090:9090
kubectl port-forward -n observability svc/grafana 30300:3000
```

**Ödev:** `82-dockerized-catalogservice` container’ına `OpenTelemetry` metrikleri ekleyip `/metrics` endpointini Prometheus’a scrape ettir.

---

## Yol Haritasını Geliştirme Önerileri

1. **Versiyonlama:** Her imaj için `demo/<servis>:vX.Y` etiketi kullanıp değişim günlüğü tut.
2. **Test Otomasyonu:** Container başına `dotnet test` veya `curl` health check script’i ekle.
3. **Gözlemlenebilirlik:** 119 ve monitoring projelerini birleştirerek gerçek metrik/log akışı oluştur.
4. **Sunum:** Her aşamayı ekran kaydıyla belgeleyerek kursiyerlerin kendi portföyüne koyabileceği bir vaka çalışması oluştur.

Bu dokümanı takip ederek kursiyerler, temel bir HTTP API’den başlayıp tam teşekküllü, gözlemlenebilir ve güvenli mikroservis platformuna kadar uzanan Docker/Kubernetes yolculuğunu uçtan uca deneyimleyebilir.

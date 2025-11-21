# On-Prem Monitoring & Logging Stack (k3d + Kubernetes)

Bu klasör, mikroservis mimarinizi lokalde (veya on-prem k3s/k3d kümelerinde) gözlemlemek için Prometheus + Grafana + Loki + Promtail kombinasyonunu içerir. Amaç, .NET tabanlı servislerinizden metrik ve log toplanmasını kolaylaştırmak ve sunumlarda hızlıca gösterilebilecek bir demo ortamı oluşturmaktır.

## İçindekiler

| Dosya/Klasör | Açıklama |
| --- | --- |
| `k3d-config.yaml` | Sunum/demo için `k3d` kümesini 1 master + 2 worker ile oluşturan tanım. Prometheus/Grafana/Loki portları host'a açılır. |
| `manifests/namespace.yaml` | `observability` namespace'i. |
| `manifests/prometheus-*` | Prometheus konfigürasyonu, deployment ve Service (NodePort 30090). |
| `manifests/grafana-*` | Grafana datasources config + deployment + Service (NodePort 30300). |
| `manifests/loki-*` | Loki config + StatefulSet + Service. |
| `manifests/promtail-*` | Promtail konfigurasyonu, RBAC ve DaemonSet. |

## 1. k3d Kümesini Oluştur

```bash
k3d cluster create --config k3d-config.yaml
export KUBECONFIG=$(k3d kubeconfig write observability)
```

- Prometheus'a `http://localhost:9090`, Grafana'ya `http://localhost:30300`, Loki API'ına `http://localhost:3100` üzerinden erişebilirsin.
- `k3d config` dosyasında Traefik devre dışı bırakıldı; istersen kendi ingress controller'ını kurabilirsin.

## 2. Observability Stack'ini Deploy Et

```bash
kubectl apply -f manifests/namespace.yaml
kubectl apply -f manifests/prometheus-configmap.yaml -f manifests/prometheus-deployment.yaml
kubectl apply -f manifests/grafana-configmap.yaml -f manifests/grafana-deployment.yaml
kubectl apply -f manifests/loki-configmap.yaml -f manifests/loki-statefulset.yaml
kubectl apply -f manifests/promtail-configmap.yaml -f manifests/promtail-daemonset.yaml
```

Durum kontrolü:
```bash
kubectl get pods -n observability
kubectl logs deploy/prometheus -n observability
```

## 3. Grafana ile Dashboad Oluştur

- Varsayılan giriş: `admin`/`admin` (ilk girişte şifreyi değiştir).
- Datasource'lar ConfigMap ile otomatik yüklendi (`Prometheus`, `Loki`).
- Önerilen dashboard'lar: 
  - `ID 15172` → .NET runtime metrikleri
  - `ID 13639` → Kubernetes cluster overview

## 4. .NET Servislerini Enstrümante Et

### Metrikler (Prometheus Scrape)

```csharp
using OpenTelemetry.Metrics;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddRuntimeInstrumentation()
        .AddPrometheusExporter());

var app = builder.Build();
app.MapPrometheusScrapingEndpoint("/metrics");
app.Run();
```

- `prometheus-configmap.yaml` içindeki `scrape_configs` anotasyonlara bakar. Endpoint'i scrape etmek için pod/servis anotasyonları ekleyin:

```yaml
metadata:
  annotations:
    prometheus.io/scrape: "true"
    prometheus.io/port: "8080"
    prometheus.io/path: "/metrics"
```

### Loglar (Loki + Promtail)

Serilog örneği:
```csharp
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.GrafanaLoki("http://loki.observability.svc.cluster.local:3100")
    .CreateLogger();
var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();
```

- Container loglarını Promtail zaten toplar. Serilog → Loki doğrudan yazmak istiyorsanız `Serilog.Sinks.GrafanaLoki` paketini ekleyin.
- Metin loglarını host üzerinde `/var/log/dotnet/*.log` altına yazıyorsanız `promtail-configmap.yaml` içindeki `dotnet-structured` job’ı bu dosyaları da Loki’ye push eder.

### Dağıtık İzleme (Opsiyonel)

`119-event-driven-final/Program.cs` dosyasındaki OpenTelemetry kurulumu ile trace’leri OTLP endpoint’ine yollayabilirsiniz. K3d üzerinde Jaeger deploy etmek isterseniz ek olarak `jaegertracing/all-in-one` imajını `observability` namespace'ine ekleyip OTLP adresini `http://jaeger-collector:4317` olarak güncelleyin.

## 5. k3d Üzerinde Port-Forward

NodePort yerine port-forward tercih etmek isterseniz:
```bash
kubectl port-forward -n observability svc/prometheus 9090:9090
kubectl port-forward -n observability svc/grafana 30300:3000
```

## 6. Demo Flow

1. `k3d` kümesini ayağa kaldırın.
2. Mikroservis manifestlerinize Prometheus anotasyonlarını ekleyip deploy edin.
3. Promtail loglarını `Loki` üzerinden sorgulayın (`{app="catalog-service"}` gibi).
4. Grafana dashboards → Prometheus metrikleri + Loki log paneli ekleyip canlı tanıtım yapın.

## Temizlik

```bash
k3d cluster delete observability
```

Bu klasörü repo'nuzda referans olarak tutup farklı ortamlar (minikube, bare-metal, gerçek on-prem) için NodePort değerlerini veya storage sınıflarını güncelleyebilirsiniz.

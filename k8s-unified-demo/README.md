# Mesh Demo (multi-service Kubernetes yigin)

Bu klasor, DotNet-Study orneklerindeki minimal API yaklasimini referans alarak iki basit .NET 8 servisi (api ve worker) ve bagli veri tabani ile tum objelerin birbiriyle iletisim kurabildigi Kubernetes ornegini iceren bir paket sunar. Ayrica baska bir namespace altinda Kafka + RabbitMQ + .NET messaging gateway servisi bulunur. Docker build adimlari, Kubernetes manifestleri ve GitHub Actions ile deploy ornegi birlikte gelir.

## Klasor yapisi
- `apps/api`: Minimal .NET API (product katalogu + worker'a cagri), Dockerfile ve csproj.
- `apps/catalog`: 46-catalog-service orneginden alinmis .NET 8 katalog servisi, Dockerfile ve csproj.
- `apps/worker`: Minimal .NET worker API (task kabul eden servis), Dockerfile ve csproj.
- `apps/messaging-gateway`: Kafka ve RabbitMQ'ya mesaj atan minimal .NET 8 API, Dockerfile ve csproj (Confluent.Kafka + RabbitMQ.Client).
- `k8s/stack.yaml`: Tum Kubernetes objelerini (Namespace, ConfigMap, Secret, Deployment, StatefulSet, Service, Gateway + HTTPRoute, HPA, NetworkPolicy, CronJob, Job, PDB, PVC, ServiceAccount, Role/RoleBinding) iceren tek dosya. Ayrica `messaging-demo` namespace'i altinda Kafka, Zookeeper, RabbitMQ ve messaging gateway yer alir.
- `.github/workflows/deploy.yml`: GHCR uzerine build/push ve kubectl apply adimlarini iceren CI/CD.

## Docker build ve push
1) GHCR icin giris yap:
```bash
echo "${GITHUB_TOKEN}" | docker login ghcr.io -u YOUR_GH_USER --password-stdin
```
2) Imge olustur ve push et:
```bash
docker build -t ghcr.io/YOUR_GH_USER/mesh-api:latest apps/api
docker build -t ghcr.io/YOUR_GH_USER/mesh-worker:latest apps/worker
docker build -t ghcr.io/YOUR_GH_USER/catalog-service:latest apps/catalog
docker build -t ghcr.io/YOUR_GH_USER/messaging-gateway:latest apps/messaging-gateway

docker push ghcr.io/YOUR_GH_USER/mesh-api:latest
docker push ghcr.io/YOUR_GH_USER/mesh-worker:latest
docker push ghcr.io/YOUR_GH_USER/catalog-service:latest
docker push ghcr.io/YOUR_GH_USER/messaging-gateway:latest
```

## Kubernetes kurulumu
Manifestler tek dosyada; iki namespace var: uygulama kismi `mesh-demo`, mesajlasma katmani `messaging-demo`. Gateway API kullanildigi icin cluster'da uyumlu bir Gateway controller ve GatewayClass (ornegin `nginx`, `istio`, `traefik`) kurulu olmalidir.
```bash
kubectl apply -f k8s/stack.yaml
kubectl get pods -n mesh-demo
kubectl get pods -n messaging-demo
```

### Iletisimi dogrulama
- API/worker:
```bash
kubectl -n mesh-demo port-forward svc/api 8080:80
curl http://localhost:8080/
curl http://localhost:8080/call-worker
```
- Worker loglarini gor:
```bash
kubectl -n mesh-demo logs -l app.kubernetes.io/name=worker
```
- Catalog servisi:
```bash
kubectl -n mesh-demo port-forward svc/catalog 8083:8083
curl http://localhost:8083/catalog/products
```
- Messaging gateway + broker'lar:
```bash
kubectl -n messaging-demo port-forward svc/messaging-gateway 8082:8082
curl -X POST http://localhost:8082/publish/kafka -H "Content-Type: application/json" -d '{"topic":"demo","message":"hello kafka"}'
curl -X POST http://localhost:8082/publish/rabbit -H "Content-Type: application/json" -d '{"queue":"demo-queue","message":"hello rabbit"}'
kubectl -n messaging-demo logs statefulset/kafka
kubectl -n messaging-demo logs statefulset/rabbitmq
```

## Yer alan Kubernetes objeleri (stack.yaml)
- `mesh-demo`: API/worker katmani icin Namespace, ConfigMap, Secret, SA+RBAC, NetworkPolicy, PVC, StatefulSet (Postgres), Deployments (api, worker, catalog), Services, Gateway + HTTPRoute, HPA, Job, CronJob, PDB.
- `messaging-demo`: Kafka ve RabbitMQ icin Zookeeper/Kafka/RabbitMQ StatefulSet + Services, ConfigMap, Secret, SA+RBAC, NetworkPolicy, PVC'ler ve messaging-gateway Deployment + Service.

## GitHub Actions (deploy.yml)
Workflow; kodu checkout eder, Buildx ile api/worker/messaging-gateway imajlarini GHCR'a push eder, kubeconfig'i `KUBE_CONFIG_DATA` secret'indan olusturup `k8s/stack.yaml` dosyasini uygular ve Deployment imajlarini gunceller. Gerekli secret:
- `KUBE_CONFIG_DATA`: base64 olarak encode edilmis kubeconfig.
- `GHCR_TOKEN` (opsiyonel): GHCR push icin, yoksa otomatik `GITHUB_TOKEN`.

## SSS / notlar
- `ghcr.io/OWNER/...` kisimlarini kendi registry/kullanici adinla degistir.
- Gateway API icin uygun controller (ornegin nginx-gateway, istio, traefik) ve ilgili GatewayClass kurulmus olmali; `/etc/hosts` veya DNS ile `mesh-demo.local` host'u cluster gateway IP'sine yonlendirilmelidir.
- HPA icin Metrics Server kurulu olmali.
- Kafka/RabbitMQ stateful set'leri icin kalici disk gerekir; local gelistirme icin default StorageClass kullanilir. Prod icin uygun storage ve guvenlik ayarlarini eklemeyi unutma.

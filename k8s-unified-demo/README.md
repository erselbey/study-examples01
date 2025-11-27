# Mesh Demo (multi-service Kubernetes yigin)

Bu klasor, DotNet-Study orneklerindeki minimal API yaklasimini referans alarak iki basit .NET 8 servisi (api ve worker) ve bagli veri tabani ile tum objelerin birbiriyle iletisim kurabildigi Kubernetes ornegini iceren bir paket sunar. Docker build adimlari, Kubernetes manifestleri ve GitHub Actions ile deploy ornegi birlikte gelir.

## Klasor yapisi
- `apps/api`: Minimal .NET API (product katalogu + worker'a cagri), Dockerfile ve csproj.
- `apps/worker`: Minimal .NET worker API (task kabul eden servis), Dockerfile ve csproj.
- `k8s/stack.yaml`: Tum Kubernetes objelerini (Namespace, ConfigMap, Secret, Deployment, StatefulSet, Service, Ingress, HPA, NetworkPolicy, CronJob, Job, PDB, PVC, ServiceAccount, Role/RoleBinding) iceren tek dosya.
- `.github/workflows/deploy.yml`: GHCR uzerine build/push ve kubectl apply adimlarini iceren CI/CD.

## Docker build ve push
1) GHCR icin giris yap:
```bash
echo "${GITHUB_TOKEN}" | docker login ghcr.io -u YOUR_GH_USER --password-stdin
```
2) Imge olustur ve push et:
```bash
IMAGE_OWNER=your-gh-user
API_TAG=ghcr.io/${IMAGE_OWNER}/mesh-api:latest
WORKER_TAG=ghcr.io/${IMAGE_OWNER}/mesh-worker:latest

docker build -t ${API_TAG} apps/api
docker build -t ${WORKER_TAG} apps/worker

docker push ${API_TAG}
docker push ${WORKER_TAG}
```

## Kubernetes kurulumu
Manifestler tek dosyada ve varsayilan namespace `mesh-demo`.
```bash
kubectl apply -f k8s/stack.yaml
kubectl get pods -n mesh-demo
```

### Iletisimi dogrulama
- Servisi localde gormek icin:
```bash
kubectl -n mesh-demo port-forward svc/api 8080:80
curl http://localhost:8080/
curl http://localhost:8080/call-worker
```
- Worker loglarini gor:
```bash
kubectl -n mesh-demo logs -l app.kubernetes.io/name=worker
```

## Yer alan Kubernetes objeleri (stack.yaml)
- Namespace, ConfigMap, Secret: ortam ayarlari ve gizli bilgileri paylasir.
- ServiceAccount, Role, RoleBinding: podlarin config/secret okuma yetkisi.
- NetworkPolicy: yalnizca ayni part-of etiketi tasiyan podlarin birbirine ulasmasi.
- PVC + Deployment: worker icin paylasilan cache alani.
- StatefulSet + Service: Postgres veri tabani kalici diskle ayaga kalkar.
- Deployments + Services: api ve worker icin ic servisler.
- Ingress: `mesh-demo.local` uzerinden api yayini (nginx ingress class).
- HPA: CPU temelli autoscale (2-5 pod).
- Job + CronJob: db tohumlama ve gece cleanup cagrisi.
- PodDisruptionBudget: api icin min 1 pod ayakta kalir.

## GitHub Actions (deploy.yml)
Workflow; kodu checkout eder, Buildx ile api/worker imajlarini GHCR'a push eder, kubeconfig'i `KUBE_CONFIG_DATA` secret'indan olusturup `k8s/stack.yaml` dosyasini uygular ve Deployment imajlarini gunceller. Gerekli secret'lar:
- `KUBE_CONFIG_DATA`: base64 olarak encode edilmis kubeconfig.
- `GHCR_TOKEN` (opsiyonel): GHCR push icin, yoksa otomatik `GITHUB_TOKEN`.

## SSS / notlar
- `ghcr.io/OWNER/...` kisimlarini kendi registry/kullanici adinla degistir.
- Ingress icin uygun controller (nginx) kurulu olmali ve DNS/hosts ayari yapilmali (`mesh-demo.local`).
- HPA icin Metrics Server kurulu olmali.

# Catalog Demo (46-catalog-service tabanli)

Bu paket yalnizca catalog-service'i Kubernetes'e kurup yayinlamak icin sade hale getirildi.

## Klasor yapisi
- `apps/catalog`: 46-catalog-service orneginden alinmis .NET 8 katalog servisi, Dockerfile ve csproj.
- `k8s/stack.yaml`: Tek namespace (`catalog-demo`), ConfigMap, ServiceAccount, NetworkPolicy, Service, Deployment, Ingress.
- `.github/workflows/deploy.yml`: GHCR'a catalog imaji push edip manifesti uygulayan CI/CD.

## Docker build ve push
1) GHCR icin giris yap:
```bash
echo "${GITHUB_TOKEN}" | docker login ghcr.io -u YOUR_GH_USER --password-stdin
```
2) Imge olustur ve push et:
```bash
docker build -t ghcr.io/YOUR_GH_USER/catalog-service:latest apps/catalog
docker push ghcr.io/YOUR_GH_USER/catalog-service:latest
```

## Kubernetes kurulumu
Tek dosya, tek namespace (`catalog-demo`). Nginx ingress gerekir.
```bash
kubectl apply -f k8s/stack.yaml
kubectl get pods -n catalog-demo
```

### Iletisimi dogrulama
```bash
kubectl -n catalog-demo port-forward svc/catalog 8083:8083
curl http://localhost:8083/catalog/products
```
Veya ingress uzerinden (hosts dosyana `catalog-demo.local` eklendigini varsayarak):
```bash
curl http://catalog-demo.local/catalog/products
```

## Yer alan Kubernetes objeleri (stack.yaml)
- `catalog-demo`: Namespace, ConfigMap, ServiceAccount, NetworkPolicy, Service, Deployment, Ingress.

## GitHub Actions (deploy.yml)
Workflow; catalog imajini GHCR'a push eder, kubeconfig'i `KUBE_CONFIG_DATA` secret'inden yazip `k8s/stack.yaml` manifestini uygular, sonra deployment imajini guncelleyip rollout bekler. Gerekli secret:
- `KUBE_CONFIG_DATA`: base64 olarak encode edilmis kubeconfig.
- `GHCR_TOKEN` (opsiyonel): GHCR push icin, yoksa otomatik `GITHUB_TOKEN`.

## SSS / notlar
- `ghcr.io/YOUR_GH_USER/...` yerlerini kendi registry/kullanici adinla degistir.
- Ingress controller'in kurulu oldugundan ve `catalog-demo.local` host'unu ilgili ingress IP'sine yonlendirdiginden emin ol.

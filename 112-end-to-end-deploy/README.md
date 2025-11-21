# 112. CI/CD + Kubernetes Deploy

Amaç: CatalogService imajını build edip registry'ye gönderdikten sonra aynı pipeline'da K8s'e deploy etmek.

## Ek Adımlar (106'nın üzerine)
```yaml
      - name: Set image
        run: kubectl set image deployment/catalog-deployment \
              catalog=ghcr.io/<org>/catalog-service:${{ github.sha }}

      - name: Wait rollout
        run: kubectl rollout status deployment/catalog-deployment

      - name: Health check
        run: kubectl run curl --rm -i --restart=Never --image=curlimages/curl \
              -- curl http://catalog-service/health
```

## Gereksinimler
- `kubectl` kurulu olmalı (örn. `azure/setup-kubectl` aksiyonu).
- Cluster erişimi için `KUBE_CONFIG` veya service account token'ı secrets olarak ekle.
- RBAC izinlerinin deployment güncellemesine izin verdiğinden emin ol.

## Opsiyonel
- Manifest uygulamak için `kubectl apply -f k8s/` adımı ekleyebilirsin.
- Rollback için `kubectl rollout undo` komutunu pipeline'a dahil et.

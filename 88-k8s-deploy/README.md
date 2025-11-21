# 88. CatalogService'i Kubernetes'e Deploy Etme

Amaç: Basit Deployment + Service manifestleriyle CatalogService'i K8s üzerinde çalıştırmak.

## Teknolojiler
- Kubernetes (minikube, kind, k3d)
- YAML manifestler

## Deployment
`k8s/deployment.yaml`
```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: catalog-deployment
spec:
  replicas: 2
  selector:
    matchLabels:
      app: catalog
  template:
    metadata:
      labels:
        app: catalog
    spec:
      containers:
        - name: catalog
          image: catalog-service:latest
          ports:
            - containerPort: 8080
```

## Service
`k8s/service.yaml`
```yaml
apiVersion: v1
kind: Service
metadata:
  name: catalog-service
spec:
  type: ClusterIP
  selector:
    app: catalog
  ports:
    - port: 80
      targetPort: 8080
```

## Uygulama
```bash
kubectl apply -f k8s/deployment.yaml -f k8s/service.yaml
kubectl port-forward svc/catalog-service 5000:80
```
`http://localhost:5000/swagger` üzerinden doğrula.

## Notlar
- Production için `imagePullSecrets`, liveness/readiness probe'ları ve resource limitleri eklemeyi unutma.

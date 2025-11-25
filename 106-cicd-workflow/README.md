# 106. CI/CD Workflow – CatalogService

Amaç: GitHub Actions pipeline'ı ile .NET projesini build, test ve container imajı olarak push etmek.

## Teknolojiler
- GitHub Actions
- Docker Registry (Docker Hub veya GHCR)
- .NET 9 SDK

## Workflow Dosyası
`.github/workflows/cicd.yaml`
```yaml
name: catalog-ci
on:
  push:
    branches: [ main ]

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: 9.0.x
      - run: dotnet restore
      - run: dotnet build --configuration Release --no-restore
      - run: dotnet test --configuration Release --no-build
      - name: Docker build & push
        uses: docker/build-push-action@v5
        with:
          context: .
          push: true
          tags: ghcr.io/<org>/catalog-service:${{ github.sha }}
```

## Notlar
- `GHCR_TOKEN`, `GHCR_USER` gibi secrets ekle.
- Docker Hub kullanıyorsan `tags: <repo>/catalog-service:${{ github.sha }}` olarak değiştir.
- Pull request trigger'ı için `pull_request` blok'u ekleyebilirsin.

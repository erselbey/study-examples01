# 82. CatalogService'i Dockerize Etme

Amaç: CatalogService'i container imajı haline getirip Docker üzerinden çalıştırmak.

## Teknolojiler
- Docker + Dockerfile
- .NET 9

## Dockerfile
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

## Komutlar
```bash
docker build -t catalog-service .
docker run -p 8080:8080 catalog-service
```

Tarayıcıdan `http://localhost:8080/swagger` adresine giderek endpointleri test et.

## İpuçları
- CI/CD pipeline'ında `--pull` flag'i ile base imajları güncel tut.
- Multi-stage build ile output boyutunu minimize ettik.

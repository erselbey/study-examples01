FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Catalog.csproj .
RUN dotnet restore Catalog.csproj

COPY . .
RUN dotnet publish Catalog.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8083
EXPOSE 8083

ENTRYPOINT ["dotnet", "Catalog.dll"]

FROM mcr.microsoft.com/dotnet/sdk:8.0

WORKDIR /src
RUN dotnet new webapi -n OrderService

WORKDIR /src/OrderService
COPY . .

RUN dotnet restore
RUN dotnet publish -c Release -o /app

CMD ["dotnet", "/app/OrderService.dll", "--urls", "http://+:5002"]

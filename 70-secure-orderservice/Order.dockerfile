FROM mcr.microsoft.com/dotnet/sdk:8.0  


WORKDIR /app
COPY . .

RUN dotnet new webapi -n OrderService

RUN cd OrderService
CMD ["dotnet", "run", "--project", "OrderService/OrderService.csproj", "--urls", "http://+:5000"]

EXPOSE 5000
#5000 portunu açtım O7
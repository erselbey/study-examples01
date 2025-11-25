FROM mcr.microsoft.com/dotnet/runtime:10.0

#Dockerfile image eklendi.

WORKDIR /app
COPY . .

#çalışma dizinini açtım bulundugum folderdan çalışma alanına kopyaladım
RUN dotnet new webapi -n CatologService 
# dotnet ile KATOLG SERVİSİ OLUŞTURDUM
RUN cd CatalogService
CMD ["dotnet", "run", "--project", "CatologService/CatologService.csproj", "--urls", "http://+:5000"]

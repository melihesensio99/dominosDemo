FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/Services/Basket/Basket.Api/BasketService.csproj", "src/Services/Basket/Basket.Api/"]
COPY ["src/BuildingBlocks/BuildingBlocks.csproj", "src/BuildingBlocks/"]
COPY ["src/Contracts/Inventory.Contracts/Inventory.Contracts.csproj", "src/Contracts/Inventory.Contracts/"]
RUN dotnet restore "src/Services/Basket/Basket.Api/BasketService.csproj"
COPY . .
WORKDIR "/src/src/Services/Basket/Basket.Api"
RUN dotnet publish "BasketService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BasketService.dll"]

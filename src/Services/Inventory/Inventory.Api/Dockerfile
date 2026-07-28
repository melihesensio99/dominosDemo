FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8004
EXPOSE 8004
EXPOSE 8007
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "InventoryService.dll"]

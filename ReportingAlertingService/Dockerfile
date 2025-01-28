# Base Image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Build Image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

# Final Image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ReportingAlertingService.dll"]

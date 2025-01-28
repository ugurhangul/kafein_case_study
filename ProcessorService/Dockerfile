# 1. Base Runtime Image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# 2. Build Image with SDK
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 3. Copy csproj Files for Each Layer
COPY ["ProcessorService.API/ProcessorService.API.csproj", "ProcessorService.API/"]
COPY ["ProcessorService.Application/ProcessorService.Application.csproj", "ProcessorService.Application/"]
COPY ["ProcessorService.Domain/ProcessorService.Domain.csproj", "ProcessorService.Domain/"]
COPY ["ProcessorService.Infrastructure/ProcessorService.Infrastructure.csproj", "ProcessorService.Infrastructure/"]

# 4. Restore Dependencies
RUN dotnet restore "ProcessorService.API/ProcessorService.API.csproj"

# 5. Copy the Entire Source Code
COPY . .

# 6. Build and Publish the Application
WORKDIR "/src/ProcessorService.API"
RUN dotnet publish "ProcessorService.API.csproj" -c Release -o /app/publish

# 7. Final Runtime Image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ProcessorService.API.dll"]

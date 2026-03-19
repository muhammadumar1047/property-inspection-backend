# Multi-stage build for .NET 8 Web API
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files for restore
COPY PropertyInspection.SaaS.sln ./
COPY PropertyInspection.API/PropertyInspection.API.csproj PropertyInspection.API/
COPY PropertyInspection.Application/PropertyInspection.Application.csproj PropertyInspection.Application/
COPY PropertyInspection.Core/PropertyInspection.Core.csproj PropertyInspection.Core/
COPY PropertyInspection.Infrastructure/PropertyInspection.Infrastructure.csproj PropertyInspection.Infrastructure/
COPY PropertyInspection.Shared/PropertyInspection.Shared.csproj PropertyInspection.Shared/

RUN dotnet restore PropertyInspection.SaaS.sln

# Copy the rest of the source code
COPY . ./

RUN dotnet publish PropertyInspection.API/PropertyInspection.API.csproj -c Release -o /app/publish /p:UseAppHost=false

# Final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Production defaults
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://0.0.0.0:5000

EXPOSE 5000

COPY --from=build /app/publish ./

ENTRYPOINT ["dotnet", "PropertyInspection.API.dll"]

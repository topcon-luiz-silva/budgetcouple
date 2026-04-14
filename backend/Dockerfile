# Multi-stage build for BudgetCouple.Api
# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["src/Api/BudgetCouple.Api.csproj", "src/Api/"]
COPY ["src/Application/BudgetCouple.Application.csproj", "src/Application/"]
COPY ["src/Domain/BudgetCouple.Domain.csproj", "src/Domain/"]
COPY ["src/Infrastructure/BudgetCouple.Infrastructure.csproj", "src/Infrastructure/"]

# Restore dependencies
RUN dotnet restore "src/Api/BudgetCouple.Api.csproj"

# Copy remaining source code
COPY . .

# Publish
WORKDIR "/src/src/Api"
RUN dotnet publish "BudgetCouple.Api.csproj" -c Release -o /app/publish --no-restore

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy published app
COPY --from=build /app/publish .

# Expose port
EXPOSE 8080

# Environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 \
  CMD dotnet /app/BudgetCouple.Api.dll --healthcheck || exit 1

# Entry point
ENTRYPOINT ["dotnet", "BudgetCouple.Api.dll"]

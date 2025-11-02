# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy solution and project files
COPY src/NotificationService/NotificationService.sln .
COPY src/NotificationService/Api/Api.csproj ./Api/
COPY src/NotificationService/Application/Application.csproj ./Application/
COPY src/NotificationService/Core/Core.csproj ./Core/
COPY src/NotificationService/Infrastructure/Infrastructure.csproj ./Infrastructure/

# Restore dependencies
RUN dotnet restore NotificationService.sln

# Copy the rest of the application files
COPY src/NotificationService/. ./

# Build the application
WORKDIR /app/Api
RUN dotnet publish -c Release -o out

# Stage 2: Create the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/Api/out ./
ENTRYPOINT ["dotnet", "Api.dll"]

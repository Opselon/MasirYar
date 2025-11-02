# Use the official .NET SDK image as a build environment
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env

# Copy the project files and restore dependencies
COPY src/IdentityService/ /app/src/IdentityService/
RUN dotnet restore /app/src/IdentityService/Api/Api.csproj

# Build the application
RUN dotnet publish /app/src/IdentityService/Api/Api.csproj -c Release -o /app/out

# Build the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "Api.dll"]

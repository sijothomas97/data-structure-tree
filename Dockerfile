# ---- Build stage ----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Restore first for better layer caching.
COPY BinaryTree.sln ./
COPY src/BinaryTree.Core/BinaryTree.Core.csproj src/BinaryTree.Core/
COPY src/BinaryTree.Api/BinaryTree.Api.csproj src/BinaryTree.Api/
RUN dotnet restore src/BinaryTree.Api/BinaryTree.Api.csproj

COPY src/ src/
RUN dotnet publish src/BinaryTree.Api/BinaryTree.Api.csproj \
    --configuration Release \
    --no-restore \
    --output /app

# ---- Runtime stage ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
USER app

ENTRYPOINT ["dotnet", "BinaryTree.Api.dll"]

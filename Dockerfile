# ─── Stage 1: Generate RSA private key ───────────────────────────────────────
FROM alpine:3.21 AS key-gen
RUN apk add --no-cache openssl && \
    openssl genrsa -out /private.pem 2048

# ─── Stage 2: Build .NET application ─────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS dotnet-build
WORKDIR /app

COPY src/WebApplication/WebApplication.csproj .
RUN dotnet restore

COPY src/WebApplication .
RUN dotnet publish -c Release -o /app/publish --no-restore

# ─── Stage 3: Build React application ────────────────────────────────────────
FROM node:20-alpine AS node-build
WORKDIR /app

COPY src/react-app/package.json src/react-app/package-lock.json ./
RUN npm install --silent

COPY src/react-app .
RUN npm run build

# ─── Stage 4: Runtime image ───────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime
WORKDIR /app

COPY --from=dotnet-build /app/publish .
COPY --from=node-build /app/build ./wwwroot
COPY --from=key-gen /private.pem ./keys/private.pem

EXPOSE 8080

CMD ["sh", "-c", "exec dotnet WebApplication.dll --urls http://*:${PORT:-8080}"]

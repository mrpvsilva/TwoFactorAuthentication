#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS build-env
WORKDIR /app

# Copiar csproj e restaurar dependencias
COPY /src/WebApplication/*.csproj .
RUN dotnet restore

# Build da aplicacao
COPY /src/WebApplication .
RUN dotnet publish -c Release -o ./publish

# the build process
FROM node:10.20-alpine as build-deps
WORKDIR /app

# install app dependencies
COPY /src/react-app/package.json .
COPY /src/react-app/package-lock.json .


RUN npm install --silent
RUN npm install react-scripts@3.4.1 -g --silent

# add app
COPY /src/react-app .

RUN  ls .

# build app
RUN npm run build

# Build da imagem
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine
WORKDIR /app
COPY --from=build-env /app/publish .
COPY --from=build-deps /app/build ./wwwroot


ENTRYPOINT ["dotnet", "WebApplication.dll"]
#CMD ASPNETCORE_URLS=http://*:$PORT dotnet WebApplication.dll
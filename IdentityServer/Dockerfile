FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5041

ENV ASPNETCORE_URLS=http://+:5041

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["IdentityServer/IdentityServer.csproj", "IdentityServer/"]
COPY ["IdentityServer/tempkey.jwk", "tempkey.jwk"]
COPY ["IdentityServer/tempkey.jwk", "IdentityServer/tempkey.jwk"]
RUN dotnet restore "IdentityServer/IdentityServer.csproj"
COPY . .
WORKDIR "/src/IdentityServer"
RUN dotnet build "IdentityServer.csproj" -c $configuration -o /app/build
FROM build AS publish
ARG configuration=Release
RUN dotnet publish "IdentityServer.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IdentityServer.dll"]

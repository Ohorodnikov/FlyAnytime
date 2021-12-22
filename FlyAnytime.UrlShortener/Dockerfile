#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 5005
EXPOSE 6005

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["FlyAnytime.UrlShortener/FlyAnytime.UrlShortener.csproj", "FlyAnytime.UrlShortener/"]
COPY ["FlyAnytime.Core/FlyAnytime.Core.csproj", "FlyAnytime.Core/"]
COPY ["FlyAnytime.Messaging/FlyAnytime.Messaging.csproj", "FlyAnytime.Messaging/"]
COPY ["FlyAnytime.Tools/FlyAnytime.Tools.csproj", "FlyAnytime.Tools/"]
RUN dotnet restore "FlyAnytime.UrlShortener/FlyAnytime.UrlShortener.csproj"
COPY . .
WORKDIR "/src/FlyAnytime.UrlShortener"
RUN dotnet build "FlyAnytime.UrlShortener.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FlyAnytime.UrlShortener.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FlyAnytime.UrlShortener.dll"]
#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["FlyAnytime.SearchEngine/FlyAnytime.SearchEngine.csproj", "FlyAnytime.SearchEngine/"]
COPY ["FlyAnytime.Core/FlyAnytime.Core.csproj", "FlyAnytime.Core/"]
COPY ["FlyAnytime.Tools/FlyAnytime.Tools.csproj", "FlyAnytime.Tools/"]
COPY ["FlyAnytime.Messaging/FlyAnytime.Messaging.csproj", "FlyAnytime.Messaging/"]
RUN dotnet restore "FlyAnytime.SearchEngine/FlyAnytime.SearchEngine.csproj"
COPY . .
WORKDIR "/src/FlyAnytime.SearchEngine"
RUN dotnet build "FlyAnytime.SearchEngine.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FlyAnytime.SearchEngine.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FlyAnytime.SearchEngine.dll"]
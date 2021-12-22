#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 5003
EXPOSE 6003

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["FlyAnytime.SearchSettings/FlyAnytime.SearchSettings.csproj", "FlyAnytime.SearchSettings/"]
COPY ["FlyAnytime.Core.Web/FlyAnytime.Core.Web.csproj", "FlyAnytime.Core.Web/"]
COPY ["FlyAnytime.Core/FlyAnytime.Core.csproj", "FlyAnytime.Core/"]
COPY ["FlyAnytime.Tools/FlyAnytime.Tools.csproj", "FlyAnytime.Tools/"]
COPY ["FlyAnytime.Messaging/FlyAnytime.Messaging.csproj", "FlyAnytime.Messaging/"]
RUN dotnet restore "FlyAnytime.SearchSettings/FlyAnytime.SearchSettings.csproj"
COPY . .
WORKDIR "/src/FlyAnytime.SearchSettings"
RUN dotnet build "FlyAnytime.SearchSettings.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FlyAnytime.SearchSettings.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FlyAnytime.SearchSettings.dll"]
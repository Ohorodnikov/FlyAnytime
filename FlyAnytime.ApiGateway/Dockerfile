#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 5001
EXPOSE 6001

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["FlyAnytime.ApiGateway/FlyAnytime.ApiGateway.csproj", "FlyAnytime.ApiGateway/"]
COPY ["FlyAnytime.Messaging/FlyAnytime.Messaging.csproj", "FlyAnytime.Messaging/"]
COPY ["FlyAnytime.Tools/FlyAnytime.Tools.csproj", "FlyAnytime.Tools/"]
COPY ["FlyAnytime.Core/FlyAnytime.Core.csproj", "FlyAnytime.Core/"]
RUN dotnet restore "FlyAnytime.ApiGateway/FlyAnytime.ApiGateway.csproj"
COPY . .
WORKDIR "/src/FlyAnytime.ApiGateway"
RUN dotnet build "FlyAnytime.ApiGateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FlyAnytime.ApiGateway.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FlyAnytime.ApiGateway.dll"]

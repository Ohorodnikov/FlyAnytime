#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 5002
EXPOSE 6002

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["FlyAnytime.Login/FlyAnytime.Login.csproj", "FlyAnytime.Login/"]
COPY ["FlyAnytime.Core.Web/FlyAnytime.Core.Web.csproj", "FlyAnytime.Core.Web/"]
COPY ["FlyAnytime.Core/FlyAnytime.Core.csproj", "FlyAnytime.Core/"]
COPY ["FlyAnytime.Tools/FlyAnytime.Tools.csproj", "FlyAnytime.Tools/"]
COPY ["FlyAnytime.Messaging/FlyAnytime.Messaging.csproj", "FlyAnytime.Messaging/"]
RUN dotnet restore "FlyAnytime.Login/FlyAnytime.Login.csproj"
COPY . .
WORKDIR "/src/FlyAnytime.Login"
RUN dotnet build "FlyAnytime.Login.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FlyAnytime.Login.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FlyAnytime.Login.dll"]
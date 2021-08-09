#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["FlyAnytime.Telegram/FlyAnytime.Telegram.csproj", "FlyAnytime.Telegram/"]
RUN dotnet restore "FlyAnytime.Telegram/FlyAnytime.Telegram.csproj"
COPY . .
WORKDIR "/src/FlyAnytime.Telegram"
RUN dotnet build "FlyAnytime.Telegram.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FlyAnytime.Telegram.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FlyAnytime.Telegram.dll"]
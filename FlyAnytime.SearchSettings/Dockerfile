#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["FlyAnytime.SearchSettings/FlyAnytime.SearchSettings.csproj", "FlyAnytime.SearchSettings/"]
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
#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:3.1 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["FlyAnytime.Scheduler/FlyAnytime.Scheduler.csproj", "FlyAnytime.Scheduler/"]
RUN dotnet restore "FlyAnytime.Scheduler/FlyAnytime.Scheduler.csproj"
COPY . .
WORKDIR "/src/FlyAnytime.Scheduler"
RUN dotnet build "FlyAnytime.Scheduler.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FlyAnytime.Scheduler.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FlyAnytime.Scheduler.dll"]
#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["FlyAnytime.Subscription/FlyAnytime.Subscription.csproj", "FlyAnytime.Subscription/"]
RUN dotnet restore "FlyAnytime.Subscription/FlyAnytime.Subscription.csproj"
COPY . .
WORKDIR "/src/FlyAnytime.Subscription"
RUN dotnet build "FlyAnytime.Subscription.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FlyAnytime.Subscription.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FlyAnytime.Subscription.dll"]
FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["MicroserviceTemplate/MicroserviceTemplate.csproj", "MicroserviceTemplate/"]
RUN dotnet restore "MicroserviceTemplate/MicroserviceTemplate.csproj"
COPY . .
WORKDIR "/src/MicroserviceTemplate"
RUN dotnet build "MicroserviceTemplate.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "MicroserviceTemplate.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "MicroserviceTemplate.dll"]
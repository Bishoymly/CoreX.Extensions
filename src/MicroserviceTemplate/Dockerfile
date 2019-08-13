FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk AS build
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
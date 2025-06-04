FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["VSMS.Stock.Application/VSMS.Stock.Application.csproj", "VSMS.Stock.Application/"]
COPY ["VSMS.Stock.Infrastructure/VSMS.Stock.Infrastructure.csproj", "VSMS.Stock.Infrastructure/"]
COPY ["VSMS.Stock.Repository/VSMS.Stock.Repository.csproj", "VSMS.Stock.Repository/"]
COPY ["VSMS.Stock.Domain/VSMS.Stock.Domain.csproj", "VSMS.Stock.Domain/"]
RUN dotnet restore "VSMS.Stock.Application/VSMS.Stock.Application.csproj"
COPY . .
WORKDIR "/src/VSMS.Stock.Application"
RUN dotnet build "./VSMS.Stock.Application.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./VSMS.Stock.Application.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VSMS.Stock.Application.dll"]

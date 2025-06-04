FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["VSMS.Company.Application/VSMS.Company.Application.csproj", "VSMS.Company.Application/"]
COPY ["VSMS.Company.Infrastructure/VSMS.Company.Infrastructure.csproj", "VSMS.Company.Infrastructure/"]
COPY ["VSMS.Company.Repository/VSMS.Company.Repository.csproj", "VSMS.Company.Repository/"]
COPY ["VSMS.Company.Domain/VSMS.Company.Domain.csproj", "VSMS.Company.Domain/"]
RUN dotnet restore "VSMS.Company.Application/VSMS.Company.Application.csproj"
COPY . .
WORKDIR "/src/VSMS.Company.Application"
RUN dotnet build "./VSMS.Company.Application.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./VSMS.Company.Application.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VSMS.Company.Application.dll"]

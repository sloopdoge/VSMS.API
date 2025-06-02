FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 6000
EXPOSE 6001

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["VSMS.Identity.Application/VSMS.Identity.Application.csproj", "VSMS.Identity.Application/"]
COPY ["VSMS.Identity.Infrastructure/VSMS.Identity.Infrastructure.csproj", "VSMS.Identity.Infrastructure/"]
COPY ["VSMS.Identity.Domain/VSMS.Identity.Domain.csproj", "VSMS.Identity.Domain/"]
COPY ["VSMS.Identity.Repository/VSMS.Identity.Repository.csproj", "VSMS.Identity.Repository/"]
RUN dotnet restore "VSMS.Identity.Application/VSMS.Identity.Application.csproj"
COPY . .
WORKDIR "/src/VSMS.Identity.Application"
RUN dotnet build "./VSMS.Identity.Application.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./VSMS.Identity.Application.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VSMS.Identity.Application.dll"]

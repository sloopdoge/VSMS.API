FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5100

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["VSMS.Application/API.Application.csproj", "VSMS.Application/"]
COPY ["VSMS.Infrastructure/API.Infrastructure.csproj", "VSMS.Infrastructure/"]
COPY ["VSMS.Repository/API.Repository.csproj", "VSMS.Repository/"]
COPY ["VSMS.Domain/API.Domain.csproj", "VSMS.Domain/"]

RUN dotnet restore "VSMS.Application/VSMS.Application.csproj"

COPY . .

WORKDIR "/src/VSMS.Application"
RUN dotnet build "VSMS.Application.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "VSMS.Application.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:5100

ENTRYPOINT ["dotnet", "VSMS.Application.dll"]
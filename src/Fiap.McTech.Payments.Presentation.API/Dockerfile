FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
RUN groupadd -r mctech && useradd -r -g mctech mctech
WORKDIR /app
EXPOSE 8081
EXPOSE 443
ARG ALLOW_SWAGGER_UI=false
ARG ALLOW_ORIGINS=*
COPY ["src/Fiap.McTech.Payments.Presentation.API/cert.pfx", "/app"]

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Fiap.McTech.Payments.Presentation.API", "Fiap.McTech.Payments.Presentation.API"]
COPY ["src/Fiap.McTech.Payments.Application", "Fiap.McTech.Payments.Application"]
COPY ["src/Fiap.McTech.Payments.Domain", "Fiap.McTech.Payments.Domain"]
COPY ["src/Fiap.McTech.Payments.Infra", "Fiap.McTech.Payments.Infra"]
COPY ["src/Fiap.McTech.Payments.CrossCutting", "Fiap.McTech.Payments.CrossCutting"]
COPY ["src/Fiap.McTech.Payments.CrossCutting.IoC", "Fiap.McTech.Payments.CrossCutting.IoC"]
COPY ["src/Fiap.McTech.Payments.ExternalService.WebAPI", "Fiap.McTech.Payments.ExternalService.WebAPI"]
RUN dotnet restore "./Fiap.McTech.Payments.Presentation.API/Fiap.McTech.Payments.Presentation.API.csproj"
WORKDIR "/src/Fiap.McTech.Payments.Presentation.API"
RUN dotnet build "./Fiap.McTech.Payments.Presentation.API.csproj" -c "$BUILD_CONFIGURATION" -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Fiap.McTech.Payments.Presentation.API.csproj" \
    -c "$BUILD_CONFIGURATION" \
    -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
USER mctech
ENTRYPOINT ["dotnet", "Fiap.McTech.Payments.Presentation.API.dll"]

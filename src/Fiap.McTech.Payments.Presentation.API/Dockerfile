# Acesse https://aka.ms/customizecontainer para saber como personalizar seu contêiner de depuração e como o Visual Studio usa este Dockerfile para criar suas imagens para uma depuração mais rápida.

# Esta fase é usada durante a execução no VS no modo rápido (Padrão para a configuração de Depuração)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
ARG ALLOW_SWAGGER_UI=false
ARG ALLOW_ORIGINS=*

# Esta fase é usada para compilar o projeto de serviço
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Fiap.McTech.Payments.Presentation.API/Fiap.McTech.Payments.Presentation.API.csproj", "src/Fiap.McTech.Payments.Presentation.API/"]
COPY ["src/Fiap.McTech.Payments.Application/Fiap.McTech.Payments.Application.csproj", "src/Fiap.McTech.Payments.Application/"]
COPY ["src/Fiap.McTech.Payments.Domain/Fiap.McTech.Payments.Domain.csproj", "src/Fiap.McTech.Payments.Domain/"]
COPY ["src/Fiap.McTech.Payments.CrossCutting/Fiap.McTech.Payments.CrossCutting.csproj", "src/Fiap.McTech.Payments.CrossCutting/"]
COPY ["src/Fiap.McTech.Payments.ExternalService.WebAPI/Fiap.McTech.Payments.ExternalService.WebAPI.csproj", "src/Fiap.McTech.Payments.ExternalService.WebAPI/"]
COPY ["src/Fiap.McTech.Payments.CrossCutting.IoC/Fiap.McTech.Payments.CrossCutting.IoC.csproj", "src/Fiap.McTech.Payments.CrossCutting.IoC/"]
COPY ["src/Fiap.McTech.Payments.Infra/Fiap.McTech.Payments.Infra.csproj", "src/Fiap.McTech.Payments.Infra/"]
RUN dotnet restore "./src/Fiap.McTech.Payments.Presentation.API/Fiap.McTech.Payments.Presentation.API.csproj"
WORKDIR "/src/src/Fiap.McTech.Payments.Presentation.API"
RUN dotnet build "./Fiap.McTech.Payments.Presentation.API.csproj" \
	-c "$BUILD_CONFIGURATION" \
	-o /app/build

# Esta fase é usada para publicar o projeto de serviço a ser copiado para a fase final
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Fiap.McTech.Payments.Presentation.API.csproj" \
	-c "$BUILD_CONFIGURATION" \
	-o /app/publish /p:UseAppHost=false

# Esta fase é usada na produção ou quando executada no VS no modo normal (padrão quando não está usando a configuração de Depuração)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Fiap.McTech.Payments.Presentation.API.dll"]
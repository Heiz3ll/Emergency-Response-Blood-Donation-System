FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BloodDonorSystem.API/BloodDonorSystem.API.csproj", "BloodDonorSystem.API/"]
COPY ["BloodDonorSystem.Application/BloodDonorSystem.Application.csproj", "BloodDonorSystem.Application/"]
COPY ["BloodDonorSystem.Domain/BloodDonorSystem.Domain.csproj", "BloodDonorSystem.Domain/"]
COPY ["BloodDonorSystem.Infrastructure/BloodDonorSystem.Infrastructure.csproj", "BloodDonorSystem.Infrastructure/"]
RUN dotnet restore "BloodDonorSystem.API/BloodDonorSystem.API.csproj"
COPY . .
WORKDIR "/src/BloodDonorSystem.API"

RUN dotnet publish \
    "BloodDonorSystem.API.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false

FROM base AS final

WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "BloodDonorSystem.API.dll"]
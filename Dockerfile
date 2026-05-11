FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/Wbn.GestaoAdm.Api/Wbn.GestaoAdm.Api.csproj", "src/Wbn.GestaoAdm.Api/"]
COPY ["src/Wbn.GestaoAdm.Application/Wbn.GestaoAdm.Application.csproj", "src/Wbn.GestaoAdm.Application/"]
COPY ["src/Wbn.GestaoAdm.Domain/Wbn.GestaoAdm.Domain.csproj", "src/Wbn.GestaoAdm.Domain/"]
COPY ["src/Wbn.GestaoAdm.Infrastructure/Wbn.GestaoAdm.Infrastructure.csproj", "src/Wbn.GestaoAdm.Infrastructure/"]

RUN dotnet restore "src/Wbn.GestaoAdm.Api/Wbn.GestaoAdm.Api.csproj"

COPY . .

WORKDIR "/src/src/Wbn.GestaoAdm.Api"
RUN dotnet build "Wbn.GestaoAdm.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Wbn.GestaoAdm.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Wbn.GestaoAdm.Api.dll"]

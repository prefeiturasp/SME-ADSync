FROM mcr.microsoft.com/dotnet/core/runtime:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["src/SME.ADSync.Worker.Service/SME.ADSync.Worker.Service.csproj", "src/SME.ADSync.Worker.Service/"]
COPY ["src/SME.ADSync.IoC/SME.ADSync.IoC.csproj", "src/SME.ADSync.IoC/"]
COPY ["src/SME.ADSync.Core/SME.ADSync.Core.csproj", "src/SME.ADSync.Core/"]
COPY ["src/SME.SyncADxCoreSSO/SME.SyncADxCoreSSO.csproj", "src/SME.SyncADxCoreSSO/"]
COPY ["src/SME.ADSync.Infra/SME.ADSync.Infra.csproj", "src/SME.ADSync.Infra/"]
COPY ["src/SME.ADSync.Servicos/SME.ADSync.Servicos.csproj", "src/SME.ADSync.Servicos/"]
COPY ["src/SME.ADSync.Background/SME.ADSync.Background.csproj", "src/SME.ADSync.Background/"]
COPY ["src/SME.ADSync.HangFire/SME.ADSync.HangFire.csproj", "src/SME.ADSync.HangFire/"]
RUN dotnet restore "src/SME.ADSync.Worker.Service/SME.ADSync.Worker.Service.csproj"
COPY . .
WORKDIR "/src/src/SME.ADSync.Worker.Service"
RUN dotnet build "SME.ADSync.Worker.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SME.ADSync.Worker.Service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SME.ADSync.Worker.Service.dll"]
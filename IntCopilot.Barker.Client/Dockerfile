FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
#USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["IntCopilot.Barker.Client/IntCopilot.Barker.Client.csproj", "IntCopilot.Barker.Client/"]
COPY ["IntCopilot.Barker.Worker/IntCopilot.Barker.Worker.csproj", "IntCopilot.Barker.Worker/"]
COPY ["IntSchool.Sharp.Core/IntSchool.Sharp.Core.csproj", "IntSchool.Sharp.Core/"]
RUN dotnet restore "IntCopilot.Barker.Client/IntCopilot.Barker.Client.csproj"
COPY . .
WORKDIR "/src/IntCopilot.Barker.Client"
RUN dotnet build "./IntCopilot.Barker.Client.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./IntCopilot.Barker.Client.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IntCopilot.Barker.Client.dll"]

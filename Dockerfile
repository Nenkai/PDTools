FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["gtapi/gtapi.csproj", "gtapi/"]
COPY ["PDTools.SimulatorInterface/PDTools.SimulatorInterface.csproj", "PDTools.SimulatorInterface/"]
COPY ["PDTools.Crypto/PDTools.Crypto.csproj", "PDTools.Crypto/"]
RUN dotnet restore "gtapi/gtapi.csproj"
COPY . .
WORKDIR "/src/gtapi"
RUN dotnet build "gtapi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "gtapi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "gtapi.dll"]

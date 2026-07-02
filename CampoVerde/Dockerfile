# 1. Fase de construcción (Build)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
# Copia el archivo de proyecto y restaura dependencias
COPY ["GanaderiaCampoVerde.csproj", "./"]
RUN dotnet restore "GanaderiaCampoVerde.csproj"

# 2. Copia todo el código y compila
COPY . .
RUN dotnet publish "GanaderiaCampoVerde.csproj" -c Release -o /app/publish

# 3. Fase final (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "GanaderiaCampoVerde.dll"]
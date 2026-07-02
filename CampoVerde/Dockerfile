# 1. Fase de construcción (Build)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
# Copia el archivo de proyecto y restaura dependencias
COPY ["CampoVerde.csproj", "./"]
RUN dotnet restore "CampoVerde.csproj"

# 2. Copia todo el código y compila
COPY . .
RUN dotnet publish "CampoVerde.csproj" -c Release -o /app/publish

# 3. Fase final (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
# Nota: Aquí el archivo .dll suele llevar el nombre del proyecto original
ENTRYPOINT ["dotnet", "CampoVerde.dll"]
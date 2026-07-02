# 1. Fase de construcción (Build)
# Cambia 8.0 por 10.0
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["CampoVerde.csproj", "./"]
RUN dotnet restore "CampoVerde.csproj"

# 2. Copia todo el código y compila
COPY . .
RUN dotnet publish "CampoVerde.csproj" -c Release -o /app/publish

# 3. Fase final (Runtime)
# Cambia 8.0 por 10.0
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CampoVerde.dll"]
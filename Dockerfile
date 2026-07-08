# ── Stage 1: Build ────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY FCG.UsersAPI/*.csproj FCG.UsersAPI/
RUN dotnet restore FCG.UsersAPI/FCG.UsersAPI.csproj

COPY . .
RUN dotnet publish FCG.UsersAPI/FCG.UsersAPI.csproj -c Release -o /app/publish

# ── Stage 2: Runtime ──────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Cria pasta de logs
RUN mkdir -p Logs

COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "FCG.UsersAPI.dll"]
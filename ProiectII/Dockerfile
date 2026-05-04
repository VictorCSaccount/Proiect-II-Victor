# ETAPA 1: Build & Publish
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["ProiectII.csproj", "./"]
RUN dotnet restore "ProiectII.csproj"

COPY . .

# Distrugem fizic mizeria locala in container inainte de build
RUN rm -rf bin/ obj/

RUN dotnet build "ProiectII.csproj" -c Release -o /app/build
RUN dotnet publish "ProiectII.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ETAPA 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080

COPY --from=build /app/publish .

COPY --from=build /src/SeedImages ./SeedImages

ENTRYPOINT ["dotnet", "ProiectII.dll"]
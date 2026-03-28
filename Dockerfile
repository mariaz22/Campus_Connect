# 1. Folosim imaginea oficială .NET SDK pentru a construi proiectul
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source

# 2. Copiem tot codul din repository în imaginea Docker
COPY . .

# 3. Restaurăm dependențele și construim aplicația
# ATENȚIE: Verifică dacă fișierul tău se numește CampusConnect.csproj în folderul src/CampusConnect
RUN dotnet restore "./src/CampusConnect/CampusConnect.Api/CampusConnect.Api.csproj"
RUN dotnet publish "./src/CampusConnect/CampusConnect.Api/CampusConnect.Api.csproj" -c Release -o /app/publish

# 4. Pregătim imaginea finală pentru rulare (mai mică și mai rapidă)
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .

# 5. Configurare porturi pentru Render
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

# 6. Comanda de start
# ATENȚIE: Dacă proiectul tău se numește altfel, înlocuiește CampusConnect.dll
ENTRYPOINT ["dotnet", "CampusConnect.Api.dll"]
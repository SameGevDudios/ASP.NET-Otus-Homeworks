FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["CorsSetup/CorsSetup.csproj", "CorsSetup/"]
RUN dotnet restore "CorsSetup/CorsSetup.csproj"
COPY . .

WORKDIR /src/CorsSetup
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR app/

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "CorsSetup.dll"]
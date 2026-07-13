FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["FarmSystemProject.csproj", "./"]
RUN dotnet restore "FarmSystemProject.csproj"

COPY . .
RUN dotnet publish "FarmSystemProject.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

RUN apt-get update && apt-get install -y --no-install-recommends \
    libfontconfig1 fontconfig libfreetype6 fonts-liberation \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "FarmSystemProject.dll"]
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

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "FarmSystemProject.dll"]
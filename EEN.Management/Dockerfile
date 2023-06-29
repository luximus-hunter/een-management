FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["EEN.Management/EEN.Management.csproj", "EEN.Management/"]
COPY ["EEN.Management/NuGet.config", "EEN.Management/"]
RUN dotnet restore --configfile "EEN.Management/NuGet.config" "EEN.Management/EEN.Management.csproj"
COPY . .
WORKDIR "/src/EEN.Management"
RUN dotnet build --configfile "NuGet.config" "EEN.Management.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "EEN.Management.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EEN.Management.dll"]

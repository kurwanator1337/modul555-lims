# API: .NET 10
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY src/Modul555.Lims.Domain/Modul555.Lims.Domain.csproj Modul555.Lims.Domain/
COPY src/Modul555.Lims.Application/Modul555.Lims.Application.csproj Modul555.Lims.Application/
COPY src/Modul555.Lims.Infrastructure/Modul555.Lims.Infrastructure.csproj Modul555.Lims.Infrastructure/
COPY src/Modul555.Lims.Api/Modul555.Lims.Api.csproj Modul555.Lims.Api/
RUN dotnet restore Modul555.Lims.Api/Modul555.Lims.Api.csproj
COPY src/ ./
RUN dotnet publish Modul555.Lims.Api/Modul555.Lims.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Modul555.Lims.Api.dll"]

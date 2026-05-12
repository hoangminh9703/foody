FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Medicare.sln ./
COPY src/Api/Medicare.Api.csproj src/Api/
COPY src/Application/Medicare.Application.csproj src/Application/
COPY src/Domain/Medicare.Domain.csproj src/Domain/
COPY src/Infrastructure/Medicare.Infrastructure.csproj src/Infrastructure/
COPY tests/Medicare.Tests.csproj tests/

RUN dotnet restore Medicare.sln

COPY . .
RUN dotnet publish src/Api/Medicare.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Medicare.Api.dll"]
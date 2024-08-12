FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env

WORKDIR /app

COPY DeliveryStore.sln ./
COPY src/web/Web.csproj src/web/
COPY src/infrastructure/Infrastructure.csproj src/infrastructure/
COPY src/application/Application.csproj src/application/
COPY src/domain/Domain.csproj src/domain/
COPY tests/Tests.csproj tests/

RUN dotnet restore

COPY . .

RUN dotnet build --configuration Release --output /out
RUN dotnet publish --configuration Release --output /out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /app

COPY --from=build-env /out .

EXPOSE 5000

ENV ASPNETCORE_URLS=http://0.0.0.0:5000

ENTRYPOINT ["dotnet", "Web.dll"]
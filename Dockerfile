# Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

COPY . .

RUN dotnet restore ./RelationshipAnalysis.sln

RUN dotnet test

RUN dotnet publish ./RelationshipAnalysis/RelationshipAnalysis.csproj -c Release -o out


# Run
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

COPY --from=build /app/out .

ENV ASPNETCORE_URLS=http://*:80

CMD dotnet App.dll

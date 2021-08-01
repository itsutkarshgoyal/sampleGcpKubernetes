#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build

WORKDIR /src

COPY ["SampleWebApp/SampleWebApp.csproj", "SampleWebApp/"]

RUN dotnet restore "SampleWebApp/SampleWebApp.csproj"

COPY . .

WORKDIR "/src/SampleWebApp"

RUN dotnet build "SampleWebApp.csproj" -c Release -o /app/build

FROM build as test
WORKDIR "/src/SampleApplicationTest/"

RUN dotnet test "SampleApplicationTest.csproj" --logger "trx;LogFileName=NAGPAPITestOutput.xml"
FROM build AS publish

#Arg variable used with Docker Build Command with a default value
ARG ASPNETCORE_ENVIRONMENT_ARG=Debug
ENV ASPNETCORE_ENVIRONMENT_ENV=$ASPNETCORE_ENVIRONMENT_ARG

RUN dotnet publish "SampleWebApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SampleWebApp.dll"]

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY . .

RUN dotnet restore "Backend/SPA.API/SPA.API.csproj"

RUN dotnet publish "Backend/SPA.API/SPA.API.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

# Install fonts and font configuration for SkiaSharp
RUN apt-get update && \
    apt-get install -y --no-install-recommends \
        fontconfig \
        fonts-dejavu-core && \
    rm -rf /var/lib/apt/lists/*

WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "SPA.API.dll"]

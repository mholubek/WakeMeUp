ARG BUILD_VERSION="dev"
ARG BUILD_ARCH

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish WakeMeUp.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0
ARG BUILD_VERSION
ARG BUILD_ARCH
LABEL \
    io.hass.version="${BUILD_VERSION}" \
    io.hass.type="addon" \
    io.hass.arch="${BUILD_ARCH}"
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
ENV TZ=Europe/Bratislava
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "WakeMeUp.dll"]

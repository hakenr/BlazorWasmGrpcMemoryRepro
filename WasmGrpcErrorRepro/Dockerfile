FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

COPY . /build/

WORKDIR /build/Server

# Restore dependencies
RUN dotnet restore -v q \
	# Dotnet publish
	&& dotnet publish -o /build/docker/output --configuration Release

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime

WORKDIR /app
COPY --from=build /build/docker/output/ ./
EXPOSE 8080
ENV ASPNETCORE_URLS http://*:8080

ENTRYPOINT ["dotnet", "WasmGrpcErrorRepro.Server.dll"]

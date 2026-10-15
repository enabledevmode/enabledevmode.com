# Build and publish with the .NET 10 SDK, then run on the smaller ASP.NET Core runtime image.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY EnableDevMode.csproj .
RUN dotnet restore
COPY . .
RUN dotnet publish EnableDevMode.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
USER $APP_UID
ENTRYPOINT ["dotnet", "EnableDevMode.dll"]

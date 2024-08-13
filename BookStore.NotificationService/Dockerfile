FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BookStore.NotificationService.Api/BookStore.NotificationService.Api.csproj", "BookStore.NotificationService.Api/"]
COPY ["BookStore.NotificationService.Commands/BookStore.NotificationService.Commands.csproj", "BookStore.NotificationService.Commands/"]
COPY ["BookStore.NotificationService.Domain/BookStore.NotificationService.Domain.csproj", "BookStore.NotificationService.Domain/"]
COPY ["BookStore.NotificationService.Infrastructure/BookStore.NotificationService.Infrastructure.csproj", "BookStore.NotificationService.Infrastructure/"]
COPY ["BookStore.NotificationService.Queries/BookStore.NotificationService.Queries.csproj", "BookStore.NotificationService.Queries/"]
RUN dotnet restore "BookStore.NotificationService.Api/BookStore.NotificationService.Api.csproj"
COPY . .
WORKDIR "/src/BookStore.NotificationService.Api"
RUN dotnet build "BookStore.NotificationService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "BookStore.NotificationService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BookStore.NotificationService.Api.dll"]

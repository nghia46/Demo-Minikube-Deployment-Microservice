#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BookStore.BorrowingService.Api/BookStore.BorrowingService.Api.csproj", "BookStore.BorrowingService.Api/"]
COPY ["BookStore.BorrowingService.Commands/BookStore.BorrowingService.Commands.csproj", "BookStore.BorrowingService.Commands/"]
COPY ["BookStore.BorrowingService.Domain/BookStore.BorrowingService.Domain.csproj", "BookStore.BorrowingService.Domain/"]
COPY ["BookStore.BorrowingService.Infrastructure/BookStore.BorrowingService.Infrastructure.csproj", "BookStore.BorrowingService.Infrastructure/"]
COPY ["BookStore.BorrowingService.Queries/BookStore.BorrowingService.Queries.csproj", "BookStore.BorrowingService.Queries/"]
RUN dotnet restore "./BookStore.BorrowingService.Api/BookStore.BorrowingService.Api.csproj"
COPY . .
WORKDIR "/src/BookStore.BorrowingService.Api"
RUN dotnet build "./BookStore.BorrowingService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BookStore.BorrowingService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BookStore.BorrowingService.Api.dll"]
# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 56167
EXPOSE 56168


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["NuGet.config", "."]
COPY ["src/4.Presentation/RpslsGameService.Api/RpslsGameService.Api.csproj", "src/4.Presentation/RpslsGameService.Api/"]
COPY ["src/2.Application/RpslsGameService.Application/RpslsGameService.Application.csproj", "src/2.Application/RpslsGameService.Application/"]
COPY ["src/1.Domain/RpslsGameService.Domain/RpslsGameService.Domain.csproj", "src/1.Domain/RpslsGameService.Domain/"]
COPY ["src/3.Infrastructure/RpslsGameService.Infrastructure/RpslsGameService.Infrastructure.csproj", "src/3.Infrastructure/RpslsGameService.Infrastructure/"]
RUN dotnet restore "./src/4.Presentation/RpslsGameService.Api/RpslsGameService.Api.csproj"
COPY . .
WORKDIR "/src/src/4.Presentation/RpslsGameService.Api"
RUN dotnet build "./RpslsGameService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./RpslsGameService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RpslsGameService.Api.dll"]
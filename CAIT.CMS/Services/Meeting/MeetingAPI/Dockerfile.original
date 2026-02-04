# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Services/Meeting/MeetingAPI/MeetingAPI.csproj", "Services/Meeting/MeetingAPI/"]
COPY ["Services/Meeting/MeetingApplication/MeetingApplication.csproj", "Services/Meeting/MeetingApplication/"]
COPY ["Services/Meeting/MeetingCore/MeetingCore.csproj", "Services/Meeting/MeetingCore/"]
COPY ["BuildingBlocks/BuildingBlocks.Contracts/BuildingBlocks.Contracts.csproj", "BuildingBlocks/BuildingBlocks.Contracts/"]
COPY ["Services/Meeting/MeetingInfrastructure/MeetingInfrastructure.csproj", "Services/Meeting/MeetingInfrastructure/"]
COPY ["BuildingBlocks/BuildingBlocks.Infrastructure/BuildingBlocks.Infrastructure.csproj", "BuildingBlocks/BuildingBlocks.Infrastructure/"]
RUN dotnet restore "./Services/Meeting/MeetingAPI/MeetingAPI.csproj"
COPY . .
WORKDIR "/src/Services/Meeting/MeetingAPI"
RUN dotnet build "./MeetingAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MeetingAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MeetingAPI.dll"]
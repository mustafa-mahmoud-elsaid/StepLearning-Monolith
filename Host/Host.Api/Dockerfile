# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Host/Host.Api/Host.Api.csproj", "Host/Host.Api/"]
COPY ["Courses/Courses.Application/Courses.Application.csproj", "Courses/Courses.Application/"]
COPY ["Courses/Courses.Domain/Courses.Domain.csproj", "Courses/Courses.Domain/"]
COPY ["Shared/StepLearning.Shared/StepLearning.Shared.csproj", "Shared/StepLearning.Shared/"]
COPY ["Courses/Courses.Infrastructure/Courses.Infrastructure.csproj", "Courses/Courses.Infrastructure/"]
COPY ["Enrollments/Enrollment.Application/Enrollment.Application.csproj", "Enrollments/Enrollment.Application/"]
COPY ["Enrollments/Enrollment.Domain/Enrollment.Domain.csproj", "Enrollments/Enrollment.Domain/"]
COPY ["Identity/Identity.Application/Identity.Application.csproj", "Identity/Identity.Application/"]
COPY ["Identity/Identity.Domain/Identity.Domain.csproj", "Identity/Identity.Domain/"]
COPY ["Commerce/Commerce.Application/Commerce.Application.csproj", "Commerce/Commerce.Application/"]
COPY ["Commerce/Commerce.Domain/Commerce.Domain.csproj", "Commerce/Commerce.Domain/"]
COPY ["Commerce/Commerce.Infrastructure/Commerce.Infrastructure.csproj", "Commerce/Commerce.Infrastructure/"]
COPY ["Notifications/Notifications.Application/Notifications.Application.csproj", "Notifications/Notifications.Application/"]
COPY ["Notifications/Notifications.Infrastructure/Notifications.Infrastructure.csproj", "Notifications/Notifications.Infrastructure/"]
COPY ["Enrollments/Enrollment.Infrastructure/Enrollment.Infrastructure.csproj", "Enrollments/Enrollment.Infrastructure/"]
COPY ["Identity/Identity.Infrastructure/Identity.Infrastructure.csproj", "Identity/Identity.Infrastructure/"]
RUN dotnet restore "./Host/Host.Api/Host.Api.csproj"
COPY . .
WORKDIR "/src/Host/Host.Api"
RUN dotnet build "./Host.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Host.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Host.Api.dll"]
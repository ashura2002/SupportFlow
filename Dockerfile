# stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

# Copy only project files
COPY ["Src/Presentation/WebApi/WebApi.csproj", "Src/Presentation/WebApi/"]
COPY ["Src/Core/Application/Application.csproj", "Src/Core/Application/"]
COPY ["Src/Infrastructure/Infrastructure/Infrastructure.csproj", "Src/Infrastructure/Infrastructure/"]
COPY ["Src/Core/Domain/Domain.csproj", "Src/Core/Domain/"]

# Restore dependencies (cached unless .csproj changes)
RUN dotnet restore "Src/Presentation/WebApi/WebApi.csproj"

# Copy the remaining source code
COPY . .

# Publish the application
# --configuration Release: Build using the Release configuration
# --output /app/publish: Store the published application files in /app/publish
# --no-restore: Skip restore because dependencies were already restored
RUN dotnet publish "Src/Presentation/WebApi/WebApi.csproj" --configuration Release --output /app/publish --no-restore



# stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

# Install GSSAPI library and remove package lists to reduce image size
RUN apt-get update \
    && apt-get install -y --no-install-recommends libgssapi-krb5-2 \ 
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app

COPY --from=build /app/publish .

# Configure ASP.NET Core to listen on port 8080
ENV ASPNETCORE_URLS=http://+:8080

# Document the port used by the ASP.NET Core application
EXPOSE 8080

# Start the ASP.NET Core application
ENTRYPOINT ["dotnet", "WebApi.dll"]
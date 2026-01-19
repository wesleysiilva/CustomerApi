# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files
COPY ["src/CustomerApi.csproj", "src/"]
COPY ["src/Directory.Build.props", "src/"]

# Restore dependencies
RUN dotnet restore "src/CustomerApi.csproj"

# Copy source code
COPY . .

# Build application
RUN dotnet build "src/CustomerApi.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "src/CustomerApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Copy published app from publish stage
COPY --from=publish /app/publish .

# Expose port
EXPOSE 80
EXPOSE 443

# Set entry point
ENTRYPOINT ["dotnet", "CustomerApi.dll"]

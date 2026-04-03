# 1. Base image for execution
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

# 2. Base image for compilation
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# 3. Copy projects and restore packages
COPY ["Mini-ETRM.slnx", "./"]
COPY ["src/Mini-ETRM.Domain/Mini-ETRM.Domain.csproj", "src/Mini-ETRM.Domain/"]
COPY ["src/Mini-ETRM.Application/Mini-ETRM.Application.csproj", "src/Mini-ETRM.Application/"]
COPY ["src/Mini-ETRM.Infrastructure/Mini-ETRM.Infrastructure.csproj", "src/Mini-ETRM.Infrastructure/"]
COPY ["src/Mini-ETRM.WebApi/Mini-ETRM.WebApi.csproj", "src/Mini-ETRM.WebApi/"]
RUN dotnet restore "Mini-ETRM.slnx"

# 4. Copy the rest of the code and build
COPY . .
WORKDIR "/src/src/Mini-ETRM.WebApi"
RUN dotnet build "Mini-ETRM.WebApi.csproj" -c Release -o /app/build

# 5. Publish optimized version
FROM build AS publish
RUN dotnet publish "Mini-ETRM.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 6. Final production image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Mini-ETRM.WebApi.dll"]
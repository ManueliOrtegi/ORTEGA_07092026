FROM mcr.microsoft.com/dotnet/sdk:10 AS build
WORKDIR /src

COPY ["src/FileProcessor.Api/FileProcessor.Api.csproj", "src/FileProcessor.Api/"]
COPY ["src/FileProcessor.Application/FileProcessor.Application.csproj", "src/FileProcessor.Application/"]
COPY ["src/FileProcessor.Domain/FileProcessor.Domain.csproj", "src/FileProcessor.Domain/"]
COPY ["src/FileProcessor.Infrastructure/FileProcessor.Infrastructure.csproj", "src/FileProcessor.Infrastructure/"]

RUN dotnet restore "src/FileProcessor.Api/FileProcessor.Api.csproj"

COPY . .

WORKDIR "/src/src/FileProcessor.Api"
RUN dotnet build "FileProcessor.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FileProcessor.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10 AS runtime
WORKDIR /app

COPY --from=publish /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "FileProcessor.Api.dll"]

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["DataGenerator.csproj", "./"]
RUN dotnet restore "DataGenerator.csproj"
COPY . .
RUN dotnet publish "DataGenerator.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "DataGenerator.dll"]
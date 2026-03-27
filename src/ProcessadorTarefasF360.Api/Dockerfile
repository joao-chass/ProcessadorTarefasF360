FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["ProcessadorTarefasF360.sln", "./"]
COPY ["src/ProcessadorTarefasF360.Api/ProcessadorTarefasF360.Api.csproj", "src/ProcessadorTarefasF360.Api/"]
COPY ["src/ProcessadorTarefasF360.Core/ProcessadorTarefasF360.Core.csproj", "src/ProcessadorTarefasF360.Core/"]

RUN dotnet restore "src/ProcessadorTarefasF360.Api/ProcessadorTarefasF360.Api.csproj"

COPY . .
RUN dotnet publish "src/ProcessadorTarefasF360.Api/ProcessadorTarefasF360.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "ProcessadorTarefasF360.Api.dll"]

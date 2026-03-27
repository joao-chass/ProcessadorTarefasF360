FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["ProcessadorTarefasF360.sln", "./"]
COPY ["src/ProcessadorTarefasF360.Worker/ProcessadorTarefasF360.Worker.csproj", "src/ProcessadorTarefasF360.Worker/"]
COPY ["src/ProcessadorTarefasF360.Core/ProcessadorTarefasF360.Core.csproj", "src/ProcessadorTarefasF360.Core/"]

RUN dotnet restore "src/ProcessadorTarefasF360.Worker/ProcessadorTarefasF360.Worker.csproj"

COPY . .
RUN dotnet publish "src/ProcessadorTarefasF360.Worker/ProcessadorTarefasF360.Worker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/runtime:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ProcessadorTarefasF360.Worker.dll"]

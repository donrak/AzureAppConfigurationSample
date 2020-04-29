FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["DemoMvc/DemoMvc.csproj", "DemoMvc/"]
RUN dotnet restore "DemoMvc/DemoMvc.csproj"
COPY . .
WORKDIR "/src/DemoMvc"
RUN dotnet build "DemoMvc.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DemoMvc.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DemoMvc.dll"]

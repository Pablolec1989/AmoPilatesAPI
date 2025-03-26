
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src


COPY ["AmoPilates/AmoPilates.csproj", "AmoPilates/"]
RUN dotnet restore "AmoPilates/AmoPilates.csproj"


COPY . .
RUN dotnet build "AmoPilates/AmoPilates.csproj" -c Release -o /app/build


RUN dotnet publish "AmoPilates/AmoPilates.csproj" -c Release -o /app/publish


FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .


EXPOSE 5055
ENV ASPNETCORE_URLS=http://*:5055
ENV ASPNETCORE_ENVIRONMENT=Development

ENTRYPOINT ["dotnet", "AmoPilates.dll"]
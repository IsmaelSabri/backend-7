# net7 conf (learn.microsoft doc)
 # https://hub.docker.com/_/microsoft-dotnet
# FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
# WORKDIR /app

# #copy csproj and restore as distinct layers
# COPY . .
# RUN dotnet restore
# RUN dotnet publish -c release -o out

# #final stage/image
# FROM mcr.microsoft.com/dotnet/aspnet:7.0
# WORKDIR /app
# COPY --from=build /app/out .
# ENTRYPOINT ["dotnet", "Homes.dll"]

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app
EXPOSE 80
EXPOSE 443

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS final
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT [ "dotnet", "Homes.dll" ]


# net6 conf
# FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
# WORKDIR /app
# EXPOSE 80

# FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
# WORKDIR /src
# COPY ["Homes.csproj", "Homes/"]
# RUN dotnet restore "Homes/Homes.csproj"
# COPY . .
# WORKDIR "/src/Homes"
# RUN dotnet build "Homes.csproj" -c Release -o /app/build

# FROM build AS publish
# RUN dotnet publish "Homes.csproj" -c Release -o /app/publish

# FROM base AS final
# WORKDIR /app
# COPY --from=publish /app/publish .
# ENTRYPOINT ["dotnet", "Homes.dll"]
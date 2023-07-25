# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /users

# copy csproj and restore as distinct layers
COPY . .
RUN dotnet restore

#WORKDIR /source/aspnetapp
RUN dotnet publish -c release -o out

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /users
COPY --from=build /users/out .
ENTRYPOINT ["dotnet", "WebApi.dll"]
FROM oven/bun:1 AS bun

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
COPY --from=bun /usr/local/bin/bun /usr/local/bin/bun
WORKDIR /src
COPY Images/Images.csproj Images/
COPY ImageWorker/ImageWorker.csproj ImageWorker/
RUN bun run dotnet restore ImageWorker/ImageWorker.csproj
COPY Images/ Images/
COPY ImageWorker/ ImageWorker/
RUN bun run dotnet publish ImageWorker/ImageWorker.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0
COPY --from=bun /usr/local/bin/bun /usr/local/bin/bun
WORKDIR /app
COPY --from=build /app/publish/ ./
ENTRYPOINT ["bun", "run", "dotnet", "ImageWorker.dll"]

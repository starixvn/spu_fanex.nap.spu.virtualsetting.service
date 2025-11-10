FROM hub.nexdev.net/base/dotnet/core/sdk:8.0-alpine-25h2 AS builder

WORKDIR /src

COPY ["./src/Fanex.NAP.SPU.VR", "Fanex.NAP.SPU.VR/"]

COPY ["./src/Fanex.NAP.SPU.VR.JobService/Fanex.NAP.SPU.VR.JobService.csproj", "Fanex.NAP.SPU.VR.JobService/"]

WORKDIR "/src/Fanex.NAP.SPU.VR.JobService"

RUN dotnet nuget add source http://nuget.nexdev.net/api/v2

RUN dotnet restore -v n

COPY ./src/Fanex.NAP.SPU.VR.JobService .

RUN dotnet publish -r linux-x64 --self-contained false -o /app

FROM hub.nexdev.net/base/dotnet/core/aspnet:8.0-alpine-25h2 AS base

WORKDIR /app

COPY --from=builder /app .

ENV TZ="America/La_Paz"

ENTRYPOINT ["dotnet", "Fanex.NAP.SPU.VR.JobService.dll"]
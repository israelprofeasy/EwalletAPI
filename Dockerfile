FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /app

COPY . ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o out


FROM mcr.microsoft.com/dotnet/sdk:5.0
WORKDIR /app
EXPOSE 80
COPY --from=build-env /app/out .
ENV ASPNETCORE_URLS=http://*:$PORT
ENTRYPOINT ["dotnet", "Ewallet.dll"]

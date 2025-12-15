FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY CreditoConstituido.slnx ./
COPY src/CreditoConstituido.Api/CreditoConstituido.Api.csproj src/CreditoConstituido.Api/
COPY src/CreditoConstituido.Application/CreditoConstituido.Application.csproj src/CreditoConstituido.Application/
COPY src/CreditoConstituido.Domain/CreditoConstituido.Domain.csproj src/CreditoConstituido.Domain/
COPY src/CreditoConstituido.Infrastructure/CreditoConstituido.Infrastructure.csproj src/CreditoConstituido.Infrastructure/

RUN dotnet restore src/CreditoConstituido.Api/CreditoConstituido.Api.csproj

COPY . .

RUN dotnet publish src/CreditoConstituido.Api/CreditoConstituido.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "CreditoConstituido.Api.dll"]

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copy sln + csproj for restore caching
COPY Invoicing.sln ./
COPY Invoicing.Api/Invoicing.Api.csproj Invoicing.Api/
COPY Invoicing.Domain/Invoicing.Domain.csproj Invoicing.Domain/
COPY Invoicing.Application/Invoicing.Application.csproj Invoicing.Application/
COPY Invoicing.Infrastructure/Invoicing.Infrastructure.csproj Invoicing.Infrastructure/

RUN dotnet restore Invoicing.Api/Invoicing.Api.csproj

# copy the rest and publish
COPY Invoicing.Api/ Invoicing.Api/
COPY Invoicing.Domain/ Invoicing.Domain/
COPY Invoicing.Application/ Invoicing.Application/
COPY Invoicing.Infrastructure/ Invoicing.Infrastructure/

WORKDIR /src/Invoicing.Api
RUN dotnet publish -c Release -o /app/publish

# ---------- runtime ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Invoicing.Api.dll"]

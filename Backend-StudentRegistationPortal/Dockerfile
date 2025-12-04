# =======================
# BUILD STAGE
# =======================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copy csproj and restore
COPY *.csproj ./
RUN dotnet restore

# Copy rest of the project files
COPY . .
RUN dotnet publish -c Release -o /app/publish

# =======================
# RUNTIME STAGE
# =======================
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 80

ENTRYPOINT ["dotnet", "UseCaseFinalSubmission.dll"]

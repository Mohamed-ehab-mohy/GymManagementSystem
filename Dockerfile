FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Directory.Packages.props ./
COPY GymManagementSystem.Domain/GymManagementSystem.Domain.csproj GymManagementSystem.Domain/
COPY GymManagementSystem.BLL/GymManagementSystem.BLL.csproj GymManagementSystem.BLL/
COPY GymManagementSystem.DAL/GymManagementSystem.DAL.csproj GymManagementSystem.DAL/
COPY GymManagementSystem.PL/GymManagementSystem.PL.csproj GymManagementSystem.PL/

RUN dotnet restore GymManagementSystem.PL/GymManagementSystem.PL.csproj

COPY . .
WORKDIR /src/GymManagementSystem.PL
RUN dotnet publish GymManagementSystem.PL.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
EXPOSE 80
EXPOSE 443

RUN apt-get update && apt-get install -y libgdiplus --no-install-recommends && rm -rf /var/lib/apt/lists/*

COPY --from=build /app .

ENTRYPOINT ["dotnet", "GymManagementSystem.PL.dll"]

#FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
#WORKDIR /app
#EXPOSE 13779
#
#FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
#WORKDIR /src
#COPY ["BookShop.csproj", "."]
#RUN dotnet restore "./BookShop.csproj"
#COPY . .
#WORKDIR "/src/."
#RUN dotnet build "BookShop.csproj" -c Release -o /app/build
#
#FROM build AS publish
#RUN dotnet publish "BookShop.csproj" -c Release -o /app/out
#
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/out .
#COPY --from=build /src/wwwroot /app/wwwroot 
#
#ENTRYPOINT ["dotnet", "BookShop.dll"]
#

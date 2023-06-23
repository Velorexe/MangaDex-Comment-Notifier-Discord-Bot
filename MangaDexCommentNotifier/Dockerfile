FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["MangaDexCommentNotifier/MangaDexCommentNotifier.csproj", "MangaDexCommentNotifier/"]
RUN dotnet restore "MangaDexCommentNotifier/MangaDexCommentNotifier.csproj"
COPY . .
WORKDIR "/src/MangaDexCommentNotifier"
RUN dotnet build "MangaDexCommentNotifier.csproj" -c Release -o /app/build

FROM build AS publish
RUN apt-get update -yq \
    && apt-get install curl gnupg -yq \
    && curl -sL https://deb.nodesource.com/setup_14.x | bash \
    && apt-get install nodejs -yq
RUN dotnet publish "MangaDexCommentNotifier.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MangaDexCommentNotifier.dll"]

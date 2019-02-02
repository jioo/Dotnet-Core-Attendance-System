# build container
FROM node:7
FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /build

# copy everything
COPY . .

# install dotnet cake tool
RUN dotnet tool install -g Cake.Tool
ENV PATH="${PATH}:/root/.dotnet/tools"

# build, restore and test
RUN dotnet cake build.cake

# runtime container
FROM microsoft/dotnet:2.2-aspnetcore-runtime AS runtime

COPY --from=build /build/dist /app
WORKDIR /app

EXPOSE 5000

RUN dotnet --list-runtimes
ENTRYPOINT ["dotnet", "Safe.Api.dll"]
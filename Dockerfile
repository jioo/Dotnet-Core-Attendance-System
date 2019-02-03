# Api Container
FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /build
COPY . .

# install dotnet cake tool
RUN dotnet tool install -g Cake.Tool
ENV PATH="${PATH}:/root/.dotnet/tools"

# build, restore and test
RUN dotnet cake build.cake --task="Api Publish"

###################################################

# Vue container
FROM node:8 AS client
WORKDIR /client
COPY . .

RUN npm install --prefix ./src/Client/

RUN npm run build --prefix ./src/Client/

###################################################

# Runtime Container
FROM microsoft/dotnet:2.2-aspnetcore-runtime AS runtime
WORKDIR /app

COPY --from=build /build/dist /app
COPY --from=client /client/src/Client/dist /app/wwwroot

WORKDIR /app

EXPOSE 5000

RUN dotnet --list-runtimes
ENTRYPOINT ["dotnet", "WebApi.dll"]
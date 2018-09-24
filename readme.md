## Dotnet-Core-Attendance-System ##

Attendance Web application using .NET Core 2.1 Web Api & Vue.js

Use login: `admin` and password: `123456`

### Installation ###
```
git clone https://github.com/jioo/Dotnet-Core-Attendance-System.git

# Navigate to Web Api folder
cd WebApi

# Restore NuGet packages
dotnet restore 

# Prepare user secret
dotnet user-secrets set "AppSecret" "__YOU_SECRET_KEY__" 

# Migrate database
dotnet ef database update 

# Start server w/ file watcher
dotnet watch run

# Navigate to Web Client folder
cd WebClient

# Install dependecies
npm install

# Start client dev w/ hot module replacement
npm run dev
```

### Features ###

* Fully separate Backend and Frontend
* Swagger for api documentation (URL: http://localhost:5000/swagger/index.html)
* CQRS Pattern (Command Query Responsibility Segregation)
* Authentication based JWT Bearer w/ Identity Framework
* Material design
* Realtime update on employee logs

### Includes ###

* [.NET Core 2.1](https://docs.microsoft.com/en-us/dotnet/core/) open-source general-purpose development platform maintained by Microsoft. 
* [Entity Framework Core](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore/) lightweight and extensible version of the popular Entity Framework data access technology.
* [AspNetCore.Identity](https://www.nuget.org/packages/Microsoft.AspNetCore.Identity/) ASP.NET Core Identity is the membership system including membership, login, and user data.
* [MediatR](https://github.com/jbogard/MediatR) Simple, unambitious mediator implementation in .NET
* [AutoMapper]() A convention-based object-object mapper.
* [Microsoft.AspNetCore.SignalR](https://www.nuget.org/packages/Microsoft.AspNetCore.SignalR/) Components for providing real-time bi-directional communication across the Web.
* [Vue.js](https://vuejs.org/) The Progressive JavaScript Framework.
* [Vuetify](https://vuetifyjs.com/en/) Material design component framework for Vue.js.
* [Vuex](https://vuex.vuejs.org/en/intro.html) State management pattern + library for Vue.js.
* [Vue-Router](https://router.vuejs.org/en/) Vue Router is the official router for Vue.js.
* [Axios](https://github.com/axios/axios) Promise based HTTP client for the browser and node.js.
* [@aspnet/signalr](https://github.com/aspnet/SignalR) JavaScript and TypeScript clients for SignalR for ASP.NET Core
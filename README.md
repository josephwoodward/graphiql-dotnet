![](https://raw.githubusercontent.com/JosephWoodward/graphiql-dotnet/master/assets/logo_128_128.png)

# GraphiQL.NET

![.NET Core](https://github.com/JosephWoodward/graphiql-dotnet/workflows/.NET%20Core/badge.svg)

GraphiQL middleware for ASP.NET Core - try the [live demo here](http://graphql.org/swapi-graphql/).

## What is GraphiQL?

GraphiQL an in-browser IDE for exploring GraphQL ([see here]( https://github.com/graphql/graphiql)). Normally in order to set GraphiQL up you need to do so via Node.

GraphiQL features include:

- Syntax highlighting.
- Intelligent type ahead of fields, arguments, types, and more.
- Real-time error highlighting and reporting.
- Automatic query completion.
- Run and inspect query results.

![GraphiQL for ASP.NET Core](https://raw.githubusercontent.com/JosephWoodward/graphiql-dotnet/master/assets/screenshot.png)

## What is GraphiQL.NET?

GraphiQL.NET saves you from needing any additional dependencies by allowing you to include the GraphiQL in-browser editor directly into your ASP.NET Core application via middleware, allowing you to explore and test your GraphQL endpoint with ease.

## Installation

The GraphiQL.NET middleware can be [found on NuGet here](https://www.nuget.org/packages/graphiql/)

You can install GraphiQL.NET by copying and pasting the following command into your Package Manager Console within Visual Studio (Tools > NuGet Package Manager > Package Manager Console).

```
Install-Package graphiql
```

Alternatively you can install it using the .NET Core CLI using the following command:

```
dotnet add package graphiql
```

## Getting Started

Once installed you can add GraphiQL.NET to your ASP.NET Core application by adding the `app.UseGraphiQl()` middleware to the `Configure` method within your `Startup.cs` file.

**Note: Be sure to call `UseGraphiQl()` before `UseMvc()`.**

```csharp

public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
{
    app.UseGraphiQl();

    app.UseMvc();
}
```

After that simply navigate to `/graphql` in your browser to start using GraphiQL.

## Configuration

By default GraphiQL lives on the aforementioned `/graphql` endpoint, however it can be changed by passing your chosen path to the `app.UseGraphiQl();` entry point method:

```csharp
app.UseGraphiQl('/whatever/graphiql');
```

You can also specify GraphiQl endpoint independent of your GraphQL API, this is especially useful if you're hosting in IIS in a virtual application (ie `myapp.com/1.0/...`) or hosting API and documentation separately.

```csharp
public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
{
    app.UseGraphiQl("/graphql", "/v1/yourapi");

    app.UseMvc();
}
```

Now navigating to `/graphql` will display the GraphiQL UI, but your GraphQL API will live under the `/v1/yourapi` route.



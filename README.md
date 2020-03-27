![](https://raw.githubusercontent.com/JosephWoodward/graphiql-dotnet/master/assets/logo_128_128.png)

# GraphiQL.NET

![.NET Core](https://github.com/JosephWoodward/graphiql-dotnet/workflows/.NET%20Core/badge.svg)

GraphiQL middleware for ASP.NET Core - try the [live demo here](http://graphql.org/swapi-graphql/).

## What is GraphiQL.NET?

GraphiQL.NET is a piece of .NET Core middleware that bundles graphiql into it saving you from managing additional frontend dependencies, whilst also giving you control over the routes it's avaialble on any provide you with a means of authentication. 

GraphiQL.NET features include:

- The full GraphiQl experience
- Customisation of GraphiQL routes
- Authentication

![GraphiQL for ASP.NET Core](https://raw.githubusercontent.com/JosephWoodward/graphiql-dotnet/master/assets/screenshot.png)


## Setup

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
    // Adding this makes graphiql UI available at /graphql 
    app.UseGraphiQl();

    app.UseMvc();
}
```

## Configuration
---
### Configure Graphiql route

By default GraphiQL lives on the `/graphql` endpoint, however this can be changed by passing your chosen path to the `app.UseGraphiQl();` entry point method:

```csharp
app.UseGraphiQl('/whatever/graphiql');
```


### Configure Graphql API address

You can also specify GraphiQl endpoint independent of your GraphQL API, this is especially useful if you're hosting in IIS in a virtual application (ie `myapp.com/1.0/...`) or hosting API and documentation separately.

```csharp
public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
{
    app.UseGraphiQl("/graphql", "/v1/yourapi");

    app.UseMvc();
}
```

Now navigating to `/graphql` will display the GraphiQL UI, but your GraphQL API will live under the `/v1/yourapi` route.

### Configuration via `IServiceCollection`

Alternatively you can configure the aforementioned routes via `IServiceCollection` within `ConfigureServices` or your `Startup.cs` file:

```csharp
//Startup.cs

public void ConfigureServices(IServiceCollection services)
{
    ...

	services.AddGraphiQl(x =>
	{
		x.GraphiQlPath = "/graphiql-ui";
		x.GraphQlApiPath = "graphql";
	});

    ...
}

public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
{
	app.UseGraphiQl();
	...
}
```

### Configuration via `ConfigureOptions<T>`

You can also use the `IConfigureOptions<T>` interface:

```csharp
// GraphiQlTestOptionsSetup.cs

internal class GraphiQlTestOptionsSetup : IConfigureOptions<GraphiQlOptions>
{
    public void Configure(GraphiQlOptions options)
    {
        options.GraphiQlPath = "/graphiql-ui";
        options.GraphQlApiPath = "graphql";
    }
}

```
Then you just have to register it with your Ioc Container:
```csharp
//Startup.cs

public void ConfigureServices(IServiceCollection services)
{
    ...
cs
services.AddTransient<IConfigureOptions<GraphiQlOptions>, GraphiQlTestOptionsSetup>();z
    ...
}
---

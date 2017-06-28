# GraphiQL.NET

GraphiQL middleware for ASP.NET Core - try the [live demo here](http://graphql.org/swapi-graphql/)

## What is GraphiQL?

GraphiQL an in-browser IDE for exploring GraphQL ([see here]( https://github.com/graphql/graphiql)). Normally in order to set GraphiQL up you need to do so via Node.

GraphiQL features include:

- Syntax highlighting
- Intelligent type ahead of fields, arguments, types, and more.
- Real-time error highlighting and reporting.
- Automatic query completion.
- Run and inspect query results.

![GraphiQL for ASP.NET Core](https://raw.githubusercontent.com/JosephWoodward/graphiql-dotnet/master/assets/screenshot.png)

## What is GraphiQL.NET?

GraphiQL.NET saves you from needing any additional dependencies by allowing you to include the GraphiQL in-browser editor directly into your ASP.NET Core application via middleware, allowing you to explore and test your GraphQL endpoint with ease.

## Getting Started

Adding GraphiQL.NET to your ASP.NET Core application couldn't be easier, simply call `app.UseGraphiQl()` from your `Configure` method in your `Startup.cs` file.

**Note: Be sure to call `UseGraphiQl()` before `UseMvc()`.**

```csharp

public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
{
    app.UseGraphiQl();

    app.UseMvc();
}
```

After that simply navigate to `/graphql` in your browser to start using GraphiQL.
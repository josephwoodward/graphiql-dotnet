using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace GraphiQl
{
    public class GraphiQlOptions
    {
        public string GraphiQlPath { get; set; } = "/graphql";

        public string GraphQlApiPath { get; set; } = "/graphql";

        public Func<HttpContext, Task<bool>> IsAuthenticated { get; set; }
    }

    public class GraphiQlOptionsSetup : IConfigureOptions<GraphiQlOptions>
    {
        public void Configure(GraphiQlOptions options)
        {
            options.IsAuthenticated = async context =>
            {
                context.Response.Clear();
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync("Boo");

                return false;
            };
        }
    }
}
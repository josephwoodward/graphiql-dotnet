using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace GraphiQl
{
    public class GraphiQlOptions
    {
        public string GraphiQlPath { get; set; }

        public string GraphQlApiPath { get; set; }

        public Func<HttpContext, Task<bool>> IsAuthenticated { get; set; }

        public GraphiQlOptions()
        {
            GraphiQlPath = "/graphql";
            GraphQlApiPath = "/graphql";
        }
    }

    public class GraphiQlOptionsSetup : IConfigureOptions<GraphiQlOptions>
    {
        public void Configure(GraphiQlOptions options)
        {
            if (options.GraphiQlPath == null)
            {
                options.GraphiQlPath = "/graphql";
            }

            if (options.GraphQlApiPath == null)
            {
                options.GraphQlApiPath = "/graphql";
            }
        }
    }
}
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace GraphiQl
{
    public class GraphiQlMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly GraphiQlOptions _options;

        public GraphiQlMiddleware(RequestDelegate next, IOptions<GraphiQlOptions> options)
        {
            _next = next;
            _options = options.Value;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.Equals(_options.GraphiQlPath, StringComparison.OrdinalIgnoreCase)
                && _options.IsAuthenticated != null
                && !await _options.IsAuthenticated.Invoke(context))
            {
                return;
            }

            await _next(context);
        }
    }
}
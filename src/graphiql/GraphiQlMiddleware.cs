using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace graphiql
{
    public class GraphiQlMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Action<GraphiQlConfig> _setConfig;

        public GraphiQlMiddleware(RequestDelegate next, Action<GraphiQlConfig> setConfig)
        {
            _next = next;
            _setConfig = setConfig;
        }

        public async Task Invoke(HttpContext context)
        {
            var config = new GraphiQlConfig();
            _setConfig(config);

            if (!HttpMethods.IsGet(context.Request.Method))
                return;

            if (!context.Request.Path.Value.Contains(config.Path))
                return;

            try
            {
                context.Response.ContentType = "text/html";
                context.Response.StatusCode = 200;
                var root = Directory.GetCurrentDirectory();
                var res = File.ReadAllBytes(root + "/middleware/content/index.html");
                var result = System.Text.Encoding.UTF8.GetString(res);
                await context.Response.WriteAsync(result);
            }
            catch (Exception e)
            {
                throw e;
            }

            await _next(context);
        }
    }
}
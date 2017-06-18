using System;
using System.IO;
using System.Reflection;
using System.Text;
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
            try
            {
                var config = new GraphiQlConfig();
                _setConfig(config);

                if (!HttpMethods.IsGet(context.Request.Method))
                    return;

                if (!context.Request.Path.Value.Contains(config.Path))
                    return;

                context.Response.ContentType = "text/html";
                context.Response.StatusCode = 200;

                var assembly = typeof(graphiql.GraphiQlExtensions).GetTypeInfo().Assembly;
                string[] names = assembly.GetManifestResourceNames();
                Stream resource = assembly.GetManifestResourceStream("graphiql.assets.index.html");

                
                using (var result = new StreamReader(resource))
                {
                    await context.Response.WriteAsync(result.ReadToEnd());
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            await _next(context);
        }
    }
}
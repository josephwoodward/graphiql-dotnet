using System;
using Microsoft.AspNetCore.Builder;

namespace graphiql
{
    public static class GraphiQlExtensions
    {
        private const string DefaultPath = "/graphql";
        public static IApplicationBuilder UseGraphiQl(this IApplicationBuilder app)
        {
            return UseGraphiQl(app, null);
        }

        private static IApplicationBuilder UseGraphiQl(this IApplicationBuilder app, string path)
        {
            return UseGraphiQlImp(app, x => x.SetPath(path ?? DefaultPath));
        }

        private static IApplicationBuilder UseGraphiQlImp(this IApplicationBuilder app, Action<GraphiQlConfig> config)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            return app.UseMiddleware<GraphiQlMiddleware>(config);
        }
    }
}

using System;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

namespace GraphiQl
{
    public static class GraphiQlExtensions
    {
        /*
        public static IServiceCollection AddGraphiQl(this IServiceCollection services)
            => services.AddGraphiQl(null);
            */

        public static IServiceCollection AddGraphiQl(this IServiceCollection services, Action<GraphiQlOptions> configure)
        {
            if (configure != null)
            {
                services.Configure(configure);
            }

            services.TryAdd(ServiceDescriptor.Transient<IConfigureOptions<GraphiQlOptions>, GraphiQlOptionsSetup>());

            return services;
        }

        public static IApplicationBuilder UseGraphiQl(this IApplicationBuilder app)
            => app.UseGraphiQl(null);

        public static IApplicationBuilder UseGraphiQl(this IApplicationBuilder app, string path)
            => app.UseGraphiQl(path, null);

        public static IApplicationBuilder UseGraphiQl(this IApplicationBuilder app, string path, string apiPath)
            => app.UseGraphiQlImp(path, apiPath);

        private static IApplicationBuilder UseGraphiQlImp(this IApplicationBuilder app, string path, string apiPath )
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            var options = app.ApplicationServices.GetService<IOptions<GraphiQlOptions>>().Value;

            var graphiQlPath = !string.IsNullOrWhiteSpace(path) ? path : options.GraphiQlPath;
            var graphQlApiPath = !string.IsNullOrWhiteSpace(apiPath) ? apiPath : options.GraphQlApiPath;

            var fileServerOptions = new FileServerOptions
            {
                RequestPath = graphiQlPath,
                FileProvider = new AssetProvider(new EmbeddedFileProvider(typeof(GraphiQlExtensions).GetTypeInfo().Assembly, "GraphiQl.assets")),
                EnableDefaultFiles = true,
                StaticFileOptions = {ContentTypeProvider = new FileExtensionContentTypeProvider()}
            };

            app.UseMiddleware<GraphiQlMiddleware>();

            app.Map($"{graphiQlPath.TrimEnd('/')}/graphql-path.js", x => WritePathJavaScript(x, graphQlApiPath));
            app.UseFileServer(fileServerOptions);

            return app;
        }

        private static void WritePathJavaScript(IApplicationBuilder app, string path)
            => app.Run(h =>
            {
                h.Response.ContentType = "application/javascript";
                return h.Response.WriteAsync($"var graphqlPath='{path}';");
            });
    }
}
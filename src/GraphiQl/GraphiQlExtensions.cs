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
        private const string DefaultPath = "/graphql";

        public static IServiceCollection AddGraphiQl(this IServiceCollection services)
        {
            return services.AddGraphiQl(null);
        }

        public static IServiceCollection AddGraphiQl(this IServiceCollection services, Action<GraphiQlOptions> configure)
        {
            if (configure != null)
            {
                services.Configure(configure);
            }

            services.TryAddTransient<IConfigureOptions<GraphiQlOptions>, GraphiQlOptionsSetup>();
            /*
            services.TryAddEnumerable(ServiceDescriptor.Transient<IConfigureOptions<GraphiQlOptions>, GraphiQlOptionsSetup>());
            */

            return services;
        }

        public static IApplicationBuilder UseGraphiQl(this IApplicationBuilder app)
        {
            return app.UseGraphiQl(DefaultPath);
        }

        public static IApplicationBuilder UseGraphiQl(this IApplicationBuilder app, string path)
        {
            return app.UseGraphiQl(path, null);
        }

        /// <param name="app"></param>
        /// <param name="path"></param>
        /// <param name="apiPath">In some scenarios it makes sense to specify the API path and file server path independently
        /// Examples: hosting in IIS in a virtual application (myapp.com/1.0/...) or hosting API and documentation separately</param>
        public static IApplicationBuilder UseGraphiQl(this IApplicationBuilder app, string path, string apiPath)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException(nameof(path));

            if (path.EndsWith("/"))
                throw new ArgumentException("GraphiQL path must not end in a slash", nameof(path));


            var filePath = $"{path}/graphql-path.js";
            var uri = !string.IsNullOrWhiteSpace(apiPath) ? apiPath : path; 
            app.Map(filePath, x => WritePathJavaScript(x, uri));

            return UseGraphiQlImp(app, x => x.SetPath(path));
        }

        private static IApplicationBuilder UseGraphiQlImp(this IApplicationBuilder app, Action<GraphiQlConfig> setConfig)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));
            if (setConfig == null)
                throw new ArgumentNullException(nameof(setConfig));
            
            var config = new GraphiQlConfig();
            setConfig(config);

            var options = new FileServerOptions
            {
                RequestPath = config.Path,
                FileProvider = CreateFileProvider(),
                EnableDefaultFiles = true,
                StaticFileOptions = {ContentTypeProvider = new FileExtensionContentTypeProvider()}
            };

            app.UseMiddleware<GraphiQlMiddleware>();
            app.UseFileServer(options);

            return app;
        }

        private static IFileProvider CreateFileProvider()
        {
            var fileProvider =
                new EmbeddedFileProvider(typeof(GraphiQlExtensions).GetTypeInfo().Assembly, "GraphiQl.assets");

            return new FileProviderProxy(fileProvider);
        }

        private static void WritePathJavaScript(IApplicationBuilder app, string path)
        {
            app.Run(h =>
            {
                h.Response.ContentType = "application/javascript";
                return h.Response.WriteAsync($"var graphqlPath='{path}';");
            });
        }
    }
}
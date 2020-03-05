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
        private const string DefaultGraphQlPath = "/graphql";

        public static IServiceCollection AddGraphiQl(this IServiceCollection services)
            => services.AddGraphiQl(null);

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
            => app.UseGraphiQl(DefaultGraphQlPath);

        public static IApplicationBuilder UseGraphiQl(this IApplicationBuilder app, string path)
            => app.UseGraphiQl(path, null);

        /// <param name="app"></param>
        /// <param name="path"></param>
        /// <param name="apiPath">In some scenarios it makes sense to specify the API path and file server path independently
        /// Examples: hosting in IIS in a virtual application (myapp.com/1.0/...) or hosting API and documentation separately</param>
        public static IApplicationBuilder UseGraphiQl(this IApplicationBuilder app, string path, string apiPath)
        {
            var options = app.ApplicationServices.GetService<IOptions<GraphiQlOptions>>().Value;

            if (options.GraphiQlPath != null && options.GraphiQlPath.EndsWith("/"))
            {
                throw new ArgumentException("GraphiQL path must not end in a slash", nameof(path));
            }

            var filePath = $"{options.GraphiQlPath}/graphql-path.js";
            var graphQlPath = !string.IsNullOrWhiteSpace(options.GraphQlApiPath) ? options.GraphQlApiPath : DefaultGraphQlPath; 
            app.Map(filePath, x => WritePathJavaScript(x, graphQlPath));

            return UseGraphiQlImp(app, options);
        }

        private static IApplicationBuilder UseGraphiQlImp(this IApplicationBuilder app, GraphiQlOptions options)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));

            var fileServerOptions = new FileServerOptions
            {
                RequestPath = options.GraphiQlPath,
                FileProvider = new EmbeddedFileProvider(typeof(GraphiQlExtensions).GetTypeInfo().Assembly, "GraphiQl.assets"),
                EnableDefaultFiles = true,
                StaticFileOptions = {ContentTypeProvider = new FileExtensionContentTypeProvider()}
            };

            app.UseMiddleware<GraphiQlMiddleware>();
            app.UseFileServer(fileServerOptions);

            return app;
        }

        private static void WritePathJavaScript(IApplicationBuilder app, string path) =>
            app.Run(h =>
            {
                h.Response.ContentType = "application/javascript";
                return h.Response.WriteAsync($"var graphqlPath='{path}';");
            });
    }
}
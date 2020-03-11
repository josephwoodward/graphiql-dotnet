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
        {
            var options = app.ApplicationServices.GetService<IOptions<GraphiQlOptions>>().Value;

            var filePath = $"{options.GraphiQlPath.TrimEnd('/')}/graphql-path.js";
            var graphQlPath = !string.IsNullOrWhiteSpace(options.GraphQlApiPath) ? options.GraphQlApiPath : DefaultGraphQlPath; 
            app.Map(filePath, x => WritePathJavaScript(x, graphQlPath));

            return UseGraphiQlImp(app, options);
        }

        private static IApplicationBuilder UseGraphiQlImp(this IApplicationBuilder app, GraphiQlOptions options)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));
            
            var provider = new EmbeddedFileProvider(typeof(GraphiQlExtensions).GetTypeInfo().Assembly, "GraphiQl.assets");
            var fileServerOptions = new FileServerOptions
            {
                RequestPath = options.GraphiQlPath,
                FileProvider = new AssetProvider(provider),
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

        [Obsolete("This overload has been marked as obsolete, please configure via IServiceCollection.AddGraphiQl(..) instead or consult the documentation", true)]
        public static IApplicationBuilder UseGraphiQl(this IApplicationBuilder app, string path)
            => throw new NotImplementedException();

        [Obsolete("This overload has been marked as obsolete, please configure via IServiceCollection.AddGraphiQl(..) instead or consult the documentation", true)]
        public static IApplicationBuilder UseGraphiQl(this IApplicationBuilder app, string path, string apiPath) 
            => throw new NotImplementedException();
    }
}
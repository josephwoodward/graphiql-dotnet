using System;
using System.Reflection;
using graphiql;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;

namespace Microsoft.AspNetCore.Builder
{
    public static class GraphiQlExtensions
    {
        public static IApplicationBuilder UseGraphiQl(this IApplicationBuilder app)
        {
            return UseGraphiQl(app, "/graphql");
        }

        public static IApplicationBuilder UseGraphiQl(this IApplicationBuilder app, string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException(nameof(path));

            path = path.StartsWith("/") ? path : "/" + path;
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

            var assembly = typeof(Microsoft.AspNetCore.Builder.GraphiQlExtensions).GetTypeInfo().Assembly;
            string[] names = assembly.GetManifestResourceNames();
                
            var fileServerOptions = new FileServerOptions
            {
                RequestPath = config.Path,
                FileProvider = new EmbeddedFileProvider(assembly, "graphiql.assets"),
                EnableDefaultFiles = true
            };

            fileServerOptions.StaticFileOptions.ContentTypeProvider = new FileExtensionContentTypeProvider();
            app.UseFileServer(fileServerOptions);

            return app;
        }
    }
}
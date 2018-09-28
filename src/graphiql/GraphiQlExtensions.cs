using System;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;

namespace GraphiQl
{
    public static class GraphiQlExtensions
    {
        private const string DefaultPath = "/graphql";
        
        public static IApplicationBuilder UseGraphiQl(this IApplicationBuilder app)
            => UseGraphiQl(app, DefaultPath);

        public static IApplicationBuilder UseGraphiQl(this IApplicationBuilder app, string path)
            => UseGraphiQlIml(app, path);
        
        private static IApplicationBuilder UseGraphiQlIml(this IApplicationBuilder app, string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException(nameof(path));

            var p = path.EndsWith("/") ? path : $"{path}/" + "graphql-path.js";
            app.Map(p, x => WritePathJavaScript(x, path));

            return UseGraphiQlImp(app, x => x.SetPath(path));
        }

        private static IApplicationBuilder UseGraphiQlImp(this IApplicationBuilder app,
            Action<GraphiQlConfig> setConfig)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));
            if (setConfig == null)
                throw new ArgumentNullException(nameof(setConfig));

            var config = new GraphiQlConfig();
            setConfig(config);

            var fileServerOptions = new FileServerOptions
            {
                RequestPath = config.Path,
                FileProvider = BuildFileProvider(),
                EnableDefaultFiles = true,
                StaticFileOptions = {ContentTypeProvider = new FileExtensionContentTypeProvider()}
            };

            app.UseFileServer(fileServerOptions);

            return app;
        }

        private static IFileProvider BuildFileProvider()
        {
            var assembly = typeof(GraphiQlExtensions).GetTypeInfo().Assembly;
            var embeddedFileProvider = new EmbeddedFileProvider(assembly, "graphiql.assets");

            var fileProvider = new CompositeFileProvider(
                embeddedFileProvider
            );

            return fileProvider;
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
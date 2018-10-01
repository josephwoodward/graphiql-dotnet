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

        /// <param name="apiPath">In some scenarios it makes sense to specify the API path and file server path independently
        /// Examples: hosting in IIS in a virtual application (myapp.com/1.0/...) or hosting API and documentation separately</param>
        public static IApplicationBuilder UseGraphiQl(this IApplicationBuilder app, string fileServerPath, string apiPath = null)
        {
            if (string.IsNullOrWhiteSpace(fileServerPath))
                throw new ArgumentException(nameof(fileServerPath));

            var p = fileServerPath.EndsWith("/") ? fileServerPath : $"{fileServerPath}/" + "graphql-path.js";
            app.Map(p, x => WritePathJavaScript(x, apiPath ?? fileServerPath));

            return UseGraphiQlImp(app, x => x.SetPath(fileServerPath));
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
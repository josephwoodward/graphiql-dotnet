using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using graphiql;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;

namespace Microsoft.AspNetCore.Builder
{
    public static class GraphiQlExtensions
    {
        internal const string DefaultGraphQLPath = "/graphql";

        public static IApplicationBuilder UseGraphiQl(this IApplicationBuilder app)
        {
            return UseGraphiQl(app, DefaultGraphQLPath);
        }

        public static IApplicationBuilder UseGraphiQl(this IApplicationBuilder app, string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException(nameof(path));

            path = path.StartsWith("/") ? path : "/" + path;
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
                FileProvider = GetFileProvider(config.Path),
                EnableDefaultFiles = true,
                StaticFileOptions = {ContentTypeProvider = new FileExtensionContentTypeProvider()}
            };

            app.UseFileServer(fileServerOptions);

            return app;
        }

        private static IFileProvider GetFileProvider(string graphqlPath)
        {
            IFileProvider fileProvider;

            var assembly = typeof(Microsoft.AspNetCore.Builder.GraphiQlExtensions).GetTypeInfo().Assembly;
            var embeddedFileProvider = new EmbeddedFileProvider(assembly, "graphiql.assets");

            if (graphqlPath.Equals(DefaultGraphQLPath, StringComparison.OrdinalIgnoreCase))
            {
                fileProvider = embeddedFileProvider;
            }
            else
            {
                string javascriptCode = $"var graphqlPath='{graphqlPath}';";

                string dir = Path.Combine(Path.GetTempPath(), "graphiql-dotnet");
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                string file = $"{dir}/graphql-path.js";

                File.WriteAllText(file, javascriptCode, Encoding.UTF8);

                var physicalFileProvider = new PhysicalFileProvider(dir);

                fileProvider = new CompositeFileProvider(
                    embeddedFileProvider,
                    physicalFileProvider
                );
            }

            return fileProvider;
        }
    }
}
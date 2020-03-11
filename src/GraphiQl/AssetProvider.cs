using System;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace GraphiQl
{
    internal class AssetProvider : IFileProvider
    {
        private readonly EmbeddedFileProvider _provider;

        public AssetProvider(EmbeddedFileProvider provider)
            => _provider = provider;

        public IFileInfo GetFileInfo(string subpath)
        {
            // Embedded resource renames hypphens in directory to underscore, this tells file provider where to find it.
            if (subpath.Equals("/es6-promise/es6-promise.min.js", StringComparison.OrdinalIgnoreCase))
            {
                subpath = "/es6_promise/es6-promise.min.js";
            }
            
            return _provider.GetFileInfo(subpath);
        }

        public IDirectoryContents GetDirectoryContents(string subpath) 
            => _provider.GetDirectoryContents(subpath);

        public IChangeToken Watch(string filter)
            => _provider.Watch(filter);
    }
}
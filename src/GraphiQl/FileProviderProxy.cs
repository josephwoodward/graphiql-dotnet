using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace GraphiQl
{
    internal sealed class FileProviderProxy : IFileProvider
    {
        private readonly IFileProvider _fileProvider;

        public FileProviderProxy(IFileProvider fileProvider)
        {
            _fileProvider = fileProvider;
        }
        
        public IFileInfo GetFileInfo(string subpath)
        {
            var fileInfo = _fileProvider.GetFileInfo(subpath);

            return fileInfo;
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
            => _fileProvider.GetDirectoryContents(subpath);

        public IChangeToken Watch(string filter)
            => _fileProvider.Watch(filter);
    }
}
using System.Threading.Tasks;
using GraphiQl.Demo;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Xunit;

namespace GraphiQl.Tests.Fixtures
{
    public class HostFixture : IAsyncLifetime
    {
        private readonly IWebHost _host;

        public string GraphiQlUri { get; } = "http://localhost:5001/graphql?query=";

        public HostFixture()
        {
            _host = WebHost.CreateDefaultBuilder()
                .UseStartup<Startup>()
                .UseKestrel()
                .UseUrls("http://*:5001")
                .Build();
        }

        public async Task InitializeAsync()
            => await _host.StartAsync().ConfigureAwait(false);

        public async Task DisposeAsync()
        {
            await _host.StopAsync().ConfigureAwait(false);
            _host.Dispose();
        }
    }
}
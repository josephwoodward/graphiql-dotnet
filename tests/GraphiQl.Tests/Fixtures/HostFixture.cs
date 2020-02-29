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

        /*
        public IWebHost Build()
        {
            var config = new ConfigurationBuilder().Build();
            var host = new WebHostBuilder()
                .UseConfiguration(config)
                .UseKestrel()
                .UseUrls("http://*:5001")
                .ConfigureServices(s =>
                {
                    s.AddMvc()
                        .AddNewtonsoftJson(o =>
                            o.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);
                    s.AddControllers()
                        .PartManager.ApplicationParts.Add(new AssemblyPart(typeof(GraphQlController).Assembly));
                })
                .Configure(app =>
                {
                    app.UseGraphiQl("/graphql");
                    app.UseRouting().UseEndpoints(routing => routing.MapControllers());
                });

            return host.Build();
        }
        */

        public async Task InitializeAsync()
            => await _host.StartAsync().ConfigureAwait(false);

        public Task DisposeAsync()
        {
            _host.Dispose();
            return Task.CompletedTask;
        }
    }
}
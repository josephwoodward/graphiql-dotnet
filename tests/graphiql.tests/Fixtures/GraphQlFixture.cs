using System.Threading.Tasks;
using GraphiQl;
using graphiql.example_core3;
using graphiql.example_core3.Controllers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace graphiql.tests.Fixtures
{
    public class GraphQlFixture
    {
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel();
                    webBuilder.UseUrls("http://*:5000");
                    webBuilder.UseStartup<Startup>();
                });
        
        public async Task CreateKestrel()
        {
            var x = new WebHostBuilder();
            var host = CreateHostBuilder(null).Build();
            await host.RunAsync();
        }
        
        public IWebHost CreateWebHostOld()
        {
            var config = new ConfigurationBuilder().Build();
            var host = new WebHostBuilder()
                .UseConfiguration(config)
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
    }
}
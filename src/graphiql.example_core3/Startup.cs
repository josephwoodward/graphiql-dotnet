using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace GraphiQl.example
{
	public class Startup
	{
		public const string GraphQlPath = "/graphql";

		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services
				.AddMvc()
				.AddNewtonsoftJson(
					options => options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
				);
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
		{
			app.UseGraphiQl(GraphQlPath);
			/* app.UseGraphiQl(GraphQlPath, "/v1/something"); */
			app.UseRouting().UseEndpoints(
				routing => routing.MapControllers()
			);
		}
	}
}
﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace GraphiQl.Demo
{
	public class Startup
	{
		public const string GraphQlPath = "/graphql";

		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			services
				.AddGraphiQl()
				.AddMvc()
				.AddNewtonsoftJson(
					options => options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
				);
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
		{
			app.UseGraphiQl();
			app.UseRouting().UseEndpoints(
				routing => routing.MapControllers()
			);
		}
	}
}
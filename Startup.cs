using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
//proptechplus 
using Config;
using DBRepository;

namespace ProjectTemplate
{
	public class Startup
	{
		//proptechplus
		private Swagger _swagger;

		public IConfiguration Configuration { get; }

		public IServiceProvider ServicesProvider {get; }

		public Startup(IConfiguration configuration, IServiceProvider servicesProvider)
		{
			Configuration = configuration;
			ServicesProvider = servicesProvider;
			//proptechplus
			this._swagger = new Swagger(configuration);
		}


		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

			//proptechplus
			this._swagger.ConfigureServices(services);

			//bind IRepository with DI container
			Console.WriteLine("Go here");
			services.AddTransient<IRepository>( s => new DB2Repository(this.Configuration["Database:ConnectionString"]));
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseMvc();

			//proptechplus
			this._swagger.Configure(app);
		}
	}
}

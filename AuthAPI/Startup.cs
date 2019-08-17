using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AuthAPI
{
	public class Startup
	{
		public Startup(IConfigurationRoot configuration, IHostingEnvironment env)
		{
			Configuration = configuration;

			var builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
				.AddEnvironmentVariables();
			this.Configuration = builder.Build();
		}

		public IConfigurationRoot Configuration { get; private set; }

		public IContainer ApplicationContainer { get; private set; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public IServiceProvider ConfigureServices(IServiceCollection services)
		{
			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
				.AddControllersAsServices();

			// Create the container builder.
			var builder = new ContainerBuilder();

			/* Register dependencies, populate the services from
			 the collection, and build the container.
			
			 Note that Populate is basically a foreach to add things
			 into Autofac that are in the collection. If you register
			 things in Autofac BEFORE Populate then the stuff in the
			 ServiceCollection can override those things; if you register
			 AFTER Populate those registrations can override things
			 in the ServiceCollection. Mix and match as needed. */
			builder.Populate(services);
			//builder.RegisterType<MyType>().As<IMyType>();
			this.ApplicationContainer = builder.Build();

			// Create the IServiceProvider based on the container.
			return new AutofacServiceProvider(this.ApplicationContainer);
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, 
			ILoggerFactory loggerFactory, IApplicationLifetime appLifetime)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseMvc();

			/* As of Autofac.Extensions.DependencyInjection 4.3.0 the AutofacDependencyResolver
			 implements IDisposable and will be disposed - along with the application container -
			 when the app stops and the WebHost disposes it.
			
			 Prior to 4.3.0, if you want to dispose of resources that have been resolved in the
			 application container, register for the "ApplicationStopped" event.
			 You can only do this if you have a direct reference to the container,
			 so it won't work with the above ConfigureContainer mechanism. */
			appLifetime.ApplicationStopped.Register(() => this.ApplicationContainer.Dispose());
		}
	}
}

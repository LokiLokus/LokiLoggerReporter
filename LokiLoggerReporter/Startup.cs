using System;
using lokiloggerreporter.Config;
using lokiloggerreporter.Extensions;
using lokiloggerreporter.Hubs;
using lokiloggerreporter.Middleware;
using lokiloggerreporter.Models;
using lokiloggerreporter.Services;
using lokiloggerreporter.Services.Implementation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;


namespace lokiloggerreporter {
	public class Startup {
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			DatabaseSettings databaseSettings = GetSettings<DatabaseSettings>("DatabaseSettings");

			if (databaseSettings.DatabaseTyp == "inmemory")
			{
				services.AddDbContext<DatabaseCtx>(opt => opt.UseInMemoryDatabase(databaseSettings.ConnectionString));

			}else if (databaseSettings.DatabaseTyp.ToLower() == "mysql")
			{
				services.AddDbContext<DatabaseCtx>(opt => opt.UseMySql(databaseSettings.ConnectionString));
			}else if (databaseSettings.DatabaseTyp.ToLower() == "sqlite")
			{
				services.AddDbContext<DatabaseCtx>(opt => opt.UseSqlite(databaseSettings.ConnectionString));
			}
			
			services.AddCors(options =>
			{
				options.AddPolicy("stuff",
					builder =>
					{
						builder.WithOrigins("*")
							.AllowAnyHeader()
							.AllowAnyMethod();
					});
			});
			services.Configure<CookiePolicyOptions>(options =>
			{
				// This lambda determines whether user consent for non-essential cookies is needed for a given request.
				options.CheckConsentNeeded = context => true;
				options.MinimumSameSitePolicy = SameSiteMode.None;
			});


			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
			});
			
			services.AddMvc().AddJsonOptions(options =>
			{
				options.SerializerSettings.ContractResolver = new DefaultContractResolver();
				options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
			}).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

			services.AddSingleton<ISettingsService, SettingService>();
			services.AddTransient<RestAnalyzeService, RestAnalyzeService>();
			services.AddTransient<LogsObtainerService, LogsObtainerService>();
			services.AddSignalR();
			services.AddUrlObtainer();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
		{
			app.UseUrlObtainer();
			app.UseCors(builder => builder
				.AllowAnyOrigin()
				.AllowAnyMethod()
				.AllowAnyHeader()
				.AllowCredentials());
			app.UseCors("default");
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
				app.UseHsts();
			}
			var settingsService= serviceProvider.GetRequiredService<ISettingsService>();
			InitHelper.SetSettings(settingsService);

			app.UseSwagger();
			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
			});

			app.UseSignalR(x =>
			{
				x.MapHub<AnalyzeHub>("/websocket");
			});
			app.UseCookiePolicy();
			app.UseMvc(routes =>
			{
				routes.MapRoute(
					"default",
					"{controller=Home}/{action=Index}/{id?}");
			});
			var ctx = serviceProvider.GetRequiredService<DatabaseCtx>();
		}
		
		private T GetSettings<T>(string section)
		{
			T setting = Configuration.GetSection(section).Get<T>();
			if (setting == null) throw new NullReferenceException(section + " is null");
			return setting;
		}

	}
}
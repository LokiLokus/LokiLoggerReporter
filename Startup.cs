using System;
using lokiloggerreporter.Config;
using lokiloggerreporter.Models;
using lokiloggerreporter.Services;
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

		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				var ctx = serviceProvider.GetRequiredService<DatabaseCtx>();
				InitHelper.CreateSource(ctx);
				//InitHelper.AddLogs(ctx);
				
				var settingsService= serviceProvider.GetRequiredService<ISettingsService>();
				
				InitHelper.SetSettings(settingsService);
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
				app.UseHsts();
			}
			app.UseLokiLogger(x =>
			{
				x.Secret = "1234";
				x.HostName = "http://localhost:5000/api/Logging/Log/b7bc5775-116d-4bba-ac7a-54e3aceac6ac";
				x.DefaultLevel = LogLevel.Debug;
			});
			app.UseSwagger();
			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
			});

			app.UseStaticFiles();
			app.UseCookiePolicy();

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					"default",
					"{controller=Home}/{action=Index}/{id?}");
			});
			
			
			
		}
		
		private T GetSettings<T>(string section)
		{
			T setting = Configuration.GetSection(section).Get<T>();
			if (setting == null) throw new NullReferenceException(section + " is null");
			return setting;
		}

	}
}
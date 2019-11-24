using System;
using lokiloggerreporter.Config;
using lokiloggerreporter.Extensions;
using lokiloggerreporter.Hubs;
using lokiloggerreporter.Middleware;
using lokiloggerreporter.Models;
using lokiloggerreporter.Services;
using lokiloggerreporter.Services.Implementation;
using lokiloggerreporter.ViewModel.User;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
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
			
			var identitySettings = IdentitySetting.Default();
			var cookieSettings = CookieSetting.Default();
			
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
            
            
            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<DatabaseCtx>()
                .AddDefaultTokenProviders();
            
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = identitySettings.Password.RequireDigit;
                options.Password.RequiredLength = identitySettings.Password.RequiredLength;
                options.Password.RequireLowercase = identitySettings.Password.RequireLowercase;
                options.Password.RequireNonAlphanumeric = identitySettings.Password.RequireNonAlphanumeric;
                options.Password.RequireUppercase = identitySettings.Password.RequireUppercase;

                options.Lockout.AllowedForNewUsers = identitySettings.Lockout.AllowedForNewUsers;
                options.Lockout.DefaultLockoutTimeSpan =
                    TimeSpan.FromMinutes(identitySettings.Lockout.DefaultLockoutTimeSpanInMins);
                options.Lockout.MaxFailedAccessAttempts = identitySettings.Lockout.MaxFailedAccessAttempts;

                options.User.RequireUniqueEmail = identitySettings.User.RequireUniqueEmail;

                options.SignIn.RequireConfirmedEmail = identitySettings.SignIn.RequireConfirmedEmail;
                options.SignIn.RequireConfirmedPhoneNumber = identitySettings.SignIn.RequireConfirmedPhoneNumber;

            });
            
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = cookieSettings.LoginPath;
                options.LogoutPath = cookieSettings.LogoutPath;
                options.AccessDeniedPath = cookieSettings.AccessDeniedPath;
                options.SlidingExpiration = cookieSettings.SlidingExpiration;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(cookieSettings.ExpireTimeSpanInMin);
            });

            services.Configure<CookiePolicyOptions>(options =>
            {
	            options.HttpOnly = HttpOnlyPolicy.None;
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.Lax;
            });
			
            services.AddAuthentication().AddCookie(options =>
            {
	            options.Cookie.HttpOnly = true;
	            options.Cookie.SecurePolicy = CookieSecurePolicy.None;
	            options.Cookie.SameSite = SameSiteMode.Lax;
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
			services.AddTransient<UserService, UserService>();
			services.AddSignalR();
			services.AddUrlObtainer();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
		{
			app.UseUrlObtainer();
			app.UseCookiePolicy();
			app.UseAuthentication();
			app.UseCors(builder => builder
				.AllowAnyOrigin()
				.AllowAnyMethod()
				.AllowAnyHeader()
				.AllowCredentials()
				.WithHeaders()
				);
			
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
			app.UseMvc(routes =>
			{
				routes.MapRoute(
					"default",
					"{controller=Home}/{action=Index}/{id?}");
			});
			var ctx = serviceProvider.GetRequiredService<DatabaseCtx>();
			var userService =  serviceProvider.GetService<UserService>();
			var roleManager =  serviceProvider.GetService<RoleManager<IdentityRole>>();
			bool existsAdmin = roleManager.RoleExistsAsync("Admin").Result;
			if (!existsAdmin)
			{
				roleManager.CreateAsync(new IdentityRole("Admin")).Wait();
			}
			if(!userService.GetAllUser().SuccessResult.Any()){
				userService.CreateUser(new UserCreateModel()
				{
					Password = "1234",
					IsAdmin = true,
					UserName = "Admin"
				}).Wait();
			}
		}
		
		private T GetSettings<T>(string section)
		{
			T setting = Configuration.GetSection(section).Get<T>();
			if (setting == null) throw new NullReferenceException(section + " is null");
			return setting;
		}

	}
}
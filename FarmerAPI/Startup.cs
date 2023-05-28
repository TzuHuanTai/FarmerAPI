using FarmerAPI.Controllers;
using FarmerAPI.Filters;
using FarmerAPI.Hubs;
using FarmerAPI.Models.SQLite;
using FarmerAPI.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using VisionPicking.BackgroundServices;

namespace FarmerAPI
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddHttpClient("systemAuth", c =>
			{
				c.BaseAddress = new Uri(Configuration["Url:SystemAuth"]);
			}).ConfigurePrimaryHttpMessageHandler(h =>
			{
				var handler = new HttpClientHandler
				{
					ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
				};
				return handler;
			});

			services.AddDbContext<GreenHouseContext>(options =>
				options.UseSqlite(Configuration.GetConnectionString("greenhouse")
			));

			services.AddCors();
			services.AddSignalR();
			services.AddMvc().AddControllersAsServices();
			//----Filter----//
			//註冊，若只個別註冊需自行在controll加上標籤[ServiceFilter(typeof(AuthorizationFilter))]
			//AddSingleton failed: AddSingleton呼叫不會new, AddTransient、AddScoped呼叫方式會new
			//services.AddScoped<AuthorizationFilter>();
			services.AddSingleton<SharedService, SharedService>();
			services.AddSingleton<RealtimeController, RealtimeController>();
			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			services.AddHostedService<DataArchivingWorker>();

			//----註冊認證，讓所有API Method可做權限控管----//
			services.AddControllers(Configuration =>
			{
				//再全域註冊Filter，ServiceFilterAttribute方式會被解析要用dependency injection，這樣就可在filter使用db功能
				//Configuration.Filters.Add(new ServiceFilterAttribute(typeof(AuthorizationFilter)));
				Configuration.MaxIAsyncEnumerableBufferLimit = 100000;
			});

			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc(
					name: "v1",
					info: new OpenApiInfo
					{
						Title = "RESTful API",
						Version = "1.2.3",
						Description = "This is ASP.NET Core RESTful API in small green house.",
						Contact = new OpenApiContact
						{
							Name = "Richard Tai",
							Email = "andy81719@gmail.com",
						}
					}
				);
				var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
				var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
				c.IncludeXmlComments(xmlPath);
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseStaticFiles();
			app.UseDefaultFiles();
			app.UseRouting();

			//----網域需要在指定條件----//
			app.UseCors(
				options => options.SetIsOriginAllowed(x => _ = true).AllowAnyMethod().AllowAnyHeader().AllowCredentials()
			);

			app.UseSwagger();
			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "RESTful API v1.0.0");
				c.DocumentTitle = "Small Greenhouse API";
			});

			//----請求進入MVC，放在所有流程最後面----//
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapHub<SignalingServer>("/SignalingServer");
				endpoints.MapHub<SensorHub>("/SensorHub");
				endpoints.MapControllers();
			});
		}
	}
}
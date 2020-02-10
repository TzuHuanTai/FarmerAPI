using System;
using System.IO;
using System.Text;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using FarmerAPI.Hubs;
using FarmerAPI.ViewModels;
using FarmerAPI.Models.SQLite;
using FarmerAPI.Models.Weather;

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
            //----驗證(AddAuthentication)Json Web Token----//
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => 
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = JwtClaimTypes.Id,
                    //RoleClaimType = JwtClaimTypes.RoleId, //停止在jwt加入角色資訊，統一用id(帳號)判斷
                    //RoleClaimType = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Actort,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["Jwt:Issuer"],      //Token頒發機構
                    ValidAudience = Configuration["Jwt:Audience"],  //Token頒發給誰
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"])) //Token簽名祕鑰
                };
                #region 必要時可使用事件do something
                //options.Events = new JwtBearerEvents()
                //{
                //    OnTokenValidated = context =>
                //    {
                //        context.HttpContext.User.Claims();
                //    }
                //};
                #endregion
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(
                    name: "v1",
                    info: new OpenApiInfo
                    {
                        Title = "RESTful API",
                        Version = "1.2.3",
                        Description = "This is ASP.NET Core RESTful API.",
                        Contact = new OpenApiContact
                        {
                            Name = "Richard",
                            Email = "andy81719@gmail.com",
                        }
                    }
                );

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            //----抓封包資訊、client IP需要註冊HttpContext功能----//
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //----連接DB，原本ConnectString移到appsettings.json----//
            services.AddDbContext<WeatherContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("MyDB")
            ));
            services.AddDbContext<GreenHouseContext>(options =>
                options.UseSqlite(Configuration.GetConnectionString("greenhouse")
            ));

            //----加入cross-origin-request-sharing----//
            services.AddCors(options=>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                               .AllowAnyMethod()
                               .AllowAnyHeader()
                               .WithExposedHeaders("Content-Disposition"); // content-disposition is *exposed* (and allowed because of AllowAnyHeader)
                    });
            });

			//----SignalR WebSocket----//
			services.AddSignalR();

			//----註冊認證，讓所有API Method可做權限控管----//
			services.AddControllers(Configuration =>
            {
                //再全域註冊Filter，ServiceFilterAttribute方式會被解析要用dependency injection，這樣就可在filter使用db功能
                //Configuration.Filters.Add(new ServiceFilterAttribute(typeof(AuthorizationFilter)));
            });

            //----Filter----//
            //註冊，若只個別註冊需自行在controll加上標籤[ServiceFilter(typeof(AuthorizationFilter))]
            //AddSingleton failed: AddSingleton呼叫不會new, AddTransient、AddScoped呼叫方式會new
            //services.AddScoped<AuthorizationFilter>();

            //----Todo: camera video----//
            //services.AddHostedService<>;
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
            app.UseCors("AllowAllOrigins");

            //----需要驗證JWT權限----//
            app.UseAuthentication();

			app.UseWebSockets();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(
                    // url: 需配合 SwaggerDoc 的 name。 "/swagger/{SwaggerDoc name}/swagger.json"
                    url: "../swagger/v1/swagger.json",
                    // description: 用於 Swagger UI 右上角選擇不同版本的 SwaggerDocument 顯示名稱使用。
                    name: "RESTful API v1.0.0"
                );
            });

            //----請求進入MVC，放在所有流程最後面----//
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<WeatherHub>("/weatherHub");
                endpoints.MapControllers();
            });
        }
    }
}

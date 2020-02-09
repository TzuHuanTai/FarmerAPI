﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FarmerAPI.Models;
using FarmerAPI.Models.Weather;
using FarmerAPI.Models.SQLite;
using Microsoft.AspNetCore.Mvc;
using FarmerAPI.Filters;
using System.Text;
using Microsoft.AspNetCore.Http;
using FarmerAPI.ViewModels;
using FarmerAPI.Hubs;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NLog.Web;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;

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

            //----抓封包資訊、client IP需要註冊HttpContext功能----//
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //----連接DB，原本ConnectString移到appsettings.json----//
            services.AddDbContext<WeatherContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("MyDB")
            ));
            services.AddDbContext<KMVContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("frudat")
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
                //AuthorizationPolicy policy = new AuthorizationPolicyBuilder()
                //                .RequireAuthenticatedUser()
                //                .Build();
                //Configuration.Filters.Add(new AuthorizeFilter(policy));

                //全域註冊Filter，靠AuthorizationFilter驗證身分權限                
                //Configuration.Filters.Add(new AuthorizationFilter());

                //再全域註冊Filter，ServiceFilterAttribute方式會被解析要用dependency injection，這樣就可在filter使用db功能
                Configuration.Filters.Add(new ServiceFilterAttribute(typeof(AuthorizationFilter)));
            });

            //----Filter----//
            //註冊，若只個別註冊需自行在controll加上標籤[ServiceFilter(typeof(AuthorizationFilter))]
            //AddSingleton failed: AddSingleton呼叫不會new, AddTransient、AddScoped呼叫方式會new
            services.AddScoped<AuthorizationFilter>();

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

			//----個別Controller註冊Middleware Filter，驗證身分權限----//
			//app.UseMiddleware<xxxxFilter>();
			//app.UseMiddleware<>

            //----請求進入MVC，放在所有流程最後面----//
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<WeatherHub>("/weatherHub");
                endpoints.MapControllers();
            });
        }
    }
}

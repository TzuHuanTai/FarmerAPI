﻿using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using FarmerAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using FarmerAPI.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Http;
using FarmerAPI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using FarmerAPI.Hubs;
using FarmerAPI.Services;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Reflection;

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
                    // name: 攸關 SwaggerDocument 的 URL 位置。
                    name: "v1",
                    // info: 是用於 SwaggerDocument 版本資訊的顯示(內容非必填)。
                    info: new Info
                    {
                        Title = "RESTful API",
                        Version = "1.2.3",
                        Description = "This is ASP.NET Core RESTful API.",
                        TermsOfService = "What is terms of service?",
                        Contact = new Contact
                        {
                            Name = "John Wu",
                            Url = "https://blog.johnwu.cc"
                        },
                        License = new License
                        {
                            Name = "CC BY-NC-SA 4.0",
                            Url = "https://creativecommons.org/licenses/by-nc-sa/4.0/"
                        }
                    }
                );

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
            services.AddDbContext<KMVContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("frudat")
            ));                               

            //----加入cross-origin-request-sharing----//
            services.AddCors(options=>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder =>
                    {
                        //CORS responses only expose these 6 headers:
                        //1.Cache-Control
                        //2.Content-Language
                        //3.Content-Type
                        //4.Expires
                        //5.Last-Modified
                        //6.Pragma
                        builder.AllowAnyOrigin()
                               .AllowAnyMethod()
                               .AllowAnyHeader()
                               .AllowCredentials()
                               .WithExposedHeaders("Content-Disposition"); // content-disposition is *exposed* (and allowed because of AllowAnyHeader)
                    });
            });

			//----加入SignalR廣播----//
			services.AddSignalR();

			//----註冊認證，讓所有API Method可做權限控管----//
			services.AddMvc(Configuration =>
            {
                //再全域註冊Filter，ServiceFilterAttribute方式會被解析要用dependency injection，這樣就可在filter使用db功能
                //Configuration.Filters.Add(new ServiceFilterAttribute(typeof(AuthorizationFilter)));
            });

            //----Filter----//
            //註冊，若只個別註冊需自行在controll加上標籤[ServiceFilter(typeof(AuthorizationFilter))]
            //AddSingleton failed: AddSingleton呼叫不會new, AddTransient、AddScoped呼叫方式會new
            //services.AddScoped<AuthorizationFilter>();

			//----MongoDB----//
			services.AddSingleton<WeatherService>();
		}

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
			//----網域需要在指定條件----//
			app.UseCors("AllowAllOrigins");

            //----需要驗證JWT權限----//
            app.UseAuthentication();

			app.UseDefaultFiles();
			app.UseStaticFiles();
			app.UseWebSockets();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(
                    // url: 需配合 SwaggerDoc 的 name。 "/swagger/{SwaggerDoc name}/swagger.json"
                    url: "/swagger/v1/swagger.json",
                    // description: 用於 Swagger UI 右上角選擇不同版本的 SwaggerDocument 顯示名稱使用。
                    name: "RESTful API v1.0.0"
                );
            });

            app.UseSignalR(routes => {
				routes.MapHub<WeatherHub>("/weatherHub");
			});

			//----請求進入MVC，放在所有流程最後面----//
			app.UseMvc();            
        }
    }
}

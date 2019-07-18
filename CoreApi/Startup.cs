using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Core.Helper;
using Core.Model.ConfigModel;
using Core.Model.WebSockets;
using CoreApi.AuthHelper;
using CoreApi.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.PlatformAbstractions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.IdentityModel.Tokens;
using NLog.Extensions.Logging;
using NLog.Web;
using Swashbuckle.AspNetCore.Swagger;

namespace CoreApi
{
    public class Startup
    {
        private IHostingEnvironment _env;
        public Startup(IHostingEnvironment env)
        {
            this._env = env;
            //Microsoft.Extensions.PlatformAbstractions.ApplicationEnvironment appEnvironment = PlatformServices.Default.Application;
            //string ContentRootPath = appEnvironment.ApplicationBasePath;
            //env.ContentRootPath = ContentRootPath;
            //env.WebRootPath = ContentRootPath;
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            this.Configuration = builder.Build();
            var test = Configuration.ToString();
            BaseConfigModel.SetBaseConfig(Configuration, env.ContentRootPath, env.WebRootPath);
        }

        /// <summary>
        /// 配置信息
        /// </summary>
        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            #region Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v0.1.0",
                    Title = "学习Swagger",
                    Description = "框架说明文档",
                    TermsOfService = "None",
                    Contact = new Swashbuckle.AspNetCore.Swagger.Contact { Name = "Learn.Swagger", Email = "king@net.com", Url = "https://www.facai.com" }
                });
                //如果不加入以下两个xml 也是可以的 但是不会对api有中文说明，使用了一下两个xml 就需要对成员使用///注释
                //本webapi的xml
                var basePath = Microsoft.DotNet.PlatformAbstractions.ApplicationEnvironment.ApplicationBasePath;
                var xmlPath = Path.Combine(basePath, "CoreApiSwagger.xml");//这个就是刚刚配置的xml文件名
                c.IncludeXmlComments(xmlPath, true);//默认的第二个参数是false，这个是controller的注释，记得修改
                //如果不引用别的类库项目，那么以上就是一个webapi项目添加swagger服务的全部
                //webapi引用model的xml 
                var xmlModelPath = Path.Combine(basePath, "CoreApiSwagger.xml");//这个就是Model层的xml文件名
                c.IncludeXmlComments(xmlPath, true);//默认的第二个参数是false，这个是controller的注释，记得修改
                c.IncludeXmlComments(xmlModelPath);

                //添加header验证信息
                //c.OperationFilter<SwaggerHeader>();
                var security = new Dictionary<string, IEnumerable<string>> { { "Bearer", new string[] { } }, };
                c.AddSecurityRequirement(security);//添加一个必须的全局安全信息，和AddSecurityDefinition方法指定的方案名称要一致，这里是Bearer。
                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = "JWT授权(数据将在请求头中进行传输) 参数结构: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",//jwt默认的参数名称
                    In = "header",//jwt默认存放Authorization信息的位置(请求头中)
                    Type = "apiKey"
                });
            });

            #endregion
            #region 认证
            services.AddAuthentication(x =>
            {
                //2、Authentication
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                JwtAuthConfigModel jwtConfig = new JwtAuthConfigModel();
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = jwtConfig.Issuer,
                    ValidAudience = jwtConfig.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtConfig.JWTSecretKey)),

                    /***********************************TokenValidationParameters的参数默认值***********************************/
                    RequireSignedTokens = true,
                    // SaveSigninToken = false,
                    // ValidateActor = false,
                    // 将下面两个参数设置为false，可以不验证Issuer和Audience，但是不建议这样做。
                    ValidateAudience = false,
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = true,
                    // 是否要求Token的Claims中必须包含 Expires
                    RequireExpirationTime = true,
                    // 允许的服务器时间偏移量
                    // ClockSkew = TimeSpan.FromSeconds(300),
                    // 是否验证Token有效期，使用当前时间与Token的Claims中的NotBefore和Expires对比
                    ValidateLifetime = true
                };
            });
            #endregion
            #region 授权
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Client", policy => policy.RequireRole("Client").Build());
                options.AddPolicy("Admin", policy => policy.RequireRole("Admin").Build());
                options.AddPolicy("AdminOrClient", policy => policy.RequireRole("Admin", "Client").Build());
            });
            services.AddDefaultIdentity<IdentityUser>().AddRoles<IdentityRole>();
            #endregion
            #region CORS 跨域
            services.AddCors(c =>
            {
                c.AddPolicy("AllowAnyOrigin", policy =>
                {
                    policy.AllowAnyOrigin()//允许任何源
                    .AllowAnyMethod()//允许任何方式
                    .AllowAnyHeader()//允许任何头
                    .AllowCredentials();//允许cookie
                });
                c.AddPolicy("AllowSpecificOrigin", policy =>
                {
                    policy.WithOrigins("http://localhost:8083")
                    .WithMethods("GET", "POST", "PUT", "DELETE")
                    .WithHeaders("authorization");
                });
            });
            #endregion
            #region Redis
            services.AddDistributedRedisCache(options =>
            {
                options.Configuration = BaseConfigModel.Configuration["Redis:ConnectionString"];
            });
            #endregion
            #region WebSockets
            services.AddSingleton<ICustomWebSocketFactory, CustomWebSocketFactory>();
            services.AddSingleton<ICustomWebSocketMessageHandler, CustomWebSocketMessageHandler>();
            #endregion

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            /* NLog */
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);//这是为了防止中文乱码
            //loggerFactory.AddNLog();//添加NLog
            //this._env.ConfigureNLog(Path.Combine("Config", "nlog.config"));//读取Nlog配置文件
            #region NLog配置
            loggerFactory.AddNLog(); // 添加NLog
            //loggerFactory.ConfigureNLog(Path.Combine("Config", "nlog.config")); // 添加Nlog.config配置文件
            loggerFactory.ConfigureNLog(Path.Combine("Config", "nlogDB.config")); //把日志文件保存到数据库配置文件
            loggerFactory.AddDebug();
            #endregion
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            #region Swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ApiHelp V1zzzz");
            });
            #endregion
            app.UseErrorHandling();
            #region TokenAuth
            app.UseMiddleware<JwtAuthorizationFilter>();
            #endregion
            #region 跨域
            app.UseCors("AllowAnyOrigin");//必须位于UserMvc之前 
            app.UseCors("AllowSpecificOrigin");//必须位于UserMvc之前 
            #endregion
            #region WebSockets
            var webSocketOptions = new Microsoft.AspNetCore.Builder.WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120),
                ReceiveBufferSize = 4 * 1024
            };
            //webSocketOptions.AllowedOrigins.Add("https://client.com");
            //webSocketOptions.AllowedOrigins.Add("https://www.client.com");
            app.UseWebSockets(webSocketOptions);
            app.UseCustomWebSocketManager();
            #endregion

            app.UseMvc();
            
        }
    }
}

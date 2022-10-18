using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SS_BlogManagement.InfraStructure;
using SS_BlogManagement.InfraStructure.ConnectionHelper;
using SS_BlogManagement.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SS_BlogManagement
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;
        public static string type;


        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            _env = env;
            _configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // configure strongly typed settings objects
            var appSettingsSection = _configuration.GetSection("AppSettings");
            services.Configure<Appsettings>(appSettingsSection);
            services.Configure<TokenLifeTime>(_configuration.GetSection("TokenLifeTime"));
            services.Configure<CryptoKey>(_configuration.GetSection("CryptoKey"));

            //DataBaseConfiguration
            services.AddTransient<IDatabaseConnectionFactory>(e =>
            {
                return new SqlConnectionFactory(_configuration.GetConnectionString("BlogManagement_Server"));
            });

            //Service Registration
            services.AddInfraStructure();
            services.AddResponseConfig();

            // configure jwt authentication
            var appSettings = appSettingsSection.Get<Appsettings>();
            var key = Encoding.UTF8.GetBytes(appSettings.Secret);
            // New JWT check
            var jwtSetting = _configuration.GetSection("Jwt");
            List<string> issuers = new List<string>();
            issuers.Add(jwtSetting.GetSection("Issuer1").Value);
            issuers.Add(jwtSetting.GetSection("Issuer2").Value);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };
            });

            //HttpContextAccess
            services.AddHttpContextAccessor();

            //Swagger
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.OperationFilter<CustomHeaderSwaggerAttribute>();

                // Set the comments path for the Swagger JSON and UI.
                //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                //c.IncludeXmlComments(xmlPath);

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme (Example: 'Bearer 12345abcdef')",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                var requirements = new Dictionary<string, IEnumerable<string>> {
                { "api key", new List<string>().AsEnumerable() }
                };
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                            {
                            Reference = new OpenApiReference
                                {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                                },
                                Scheme = "oauth2",
                                Name = "Bearer",
                                In = ParameterLocation.Header,
                            },
                        new List<string>()
                    }
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SS_BlogManagement v1"));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();


            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
    //public class Startup
    //{
    //    private readonly IWebHostEnvironment _env;
    //    private readonly IConfiguration _configuration;

    //    public Startup(IWebHostEnvironment env, IConfiguration configuration)
    //    {
    //        _env = env;
    //        _configuration = configuration;
    //    }

    //    // This method gets called by the runtime. Use this method to add services to the container.
    //    public void ConfigureServices(IServiceCollection services)
    //    {
    //        // configure strongly typed settings objects
    //        var appSettingsSection = _configuration.GetSection("AppSettings");
    //        services.Configure<Appsettings>(appSettingsSection);

    //        var cryptoKeysSection = _configuration.GetSection("CryptoKey");
    //        services.Configure<CryptoKey>(cryptoKeysSection);

    //        var tokenLifeTimeSection = _configuration.GetSection("TokenLifeTime");
    //        services.Configure<TokenLifeTime>(tokenLifeTimeSection);

    //        //DataBaseConfiguration
    //        services.AddTransient<IDatabaseConnectionFactory>(e =>
    //        {
    //            return new SqlConnectionFactory(_configuration.GetConnectionString("BlogManagement_Server"));
    //        });

    //        //Service Registration
    //        services.AddInfraStructure();
    //        services.AddResponseConfig();

    //        // configure jwt authentication
    //        var appSettings = appSettingsSection.Get<Appsettings>();
    //        var key = Encoding.UTF8.GetBytes(appSettings.Secret);
    //        // New JWT check
    //        var jwtSetting = _configuration.GetSection("Jwt");
    //        List<string> issuers = new List<string>();
    //        issuers.Add(jwtSetting.GetSection("Issuer1").Value);
    //        issuers.Add(jwtSetting.GetSection("Issuer2").Value);

    //        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
    //        {
    //            options.RequireHttpsMetadata = false;
    //            options.SaveToken = true;
    //            options.TokenValidationParameters = new TokenValidationParameters
    //            {
    //                ValidateIssuerSigningKey = true,
    //                IssuerSigningKey = new SymmetricSecurityKey(key),
    //                ValidateIssuer = false,
    //                ValidateAudience = false,
    //                ClockSkew = TimeSpan.Zero
    //            };
    //        });

    //        //HttpContextAccess
    //        services.AddHttpContextAccessor();

    //        //Swagger
    //        services.AddControllers();
    //        services.AddSwaggerGen(c =>
    //        {
    //            //c.SwaggerDoc("v1", new Info { Title = "You api title", Version = "v1" });

    //            // Set the comments path for the Swagger JSON and UI.
    //            //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    //            //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    //            //c.IncludeXmlComments(xmlPath);

    //            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    //            {
    //                Description = @"JWT Authorization header using the Bearer scheme (Example: 'Bearer 12345abcdef')",
    //                Name = "Authorization",
    //                In = ParameterLocation.Header,
    //                Type = SecuritySchemeType.ApiKey,
    //                Scheme = "Bearer"
    //            });
    //            var requirements = new Dictionary<string, IEnumerable<string>> {
    //            { "api key", new List<string>().AsEnumerable() }
    //            };
    //            c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    //            {
    //                {
    //                    new OpenApiSecurityScheme
    //                        {
    //                        Reference = new OpenApiReference
    //                            {
    //                            Type = ReferenceType.SecurityScheme,
    //                            Id = "Bearer"
    //                            },
    //                            Scheme = "oauth2",
    //                            Name = "Bearer",
    //                            In = ParameterLocation.Header,
    //                        },
    //                    new List<string>()
    //                }
    //            });
    //        });
    //    }

    //    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    //    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    //    {
    //        //if (env.IsDevelopment())
    //        //{
    //        //    app.UseDeveloperExceptionPage();
    //        //    app.UseSwagger();
    //        //    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SS_BlogManagement v1"));
    //        //}
    //        app.UseSwagger();
    //        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SS_BlogManagement v1"));
    //        app.UseHttpsRedirection();

    //        app.UseRouting();

    //        // configure jwt authentication
    //        app.UseAuthorization();

    //        app.UseAuthentication();

    //        app.UseEndpoints(endpoints =>
    //        {
    //            endpoints.MapControllers();
    //        });
    //    }
    //}
}

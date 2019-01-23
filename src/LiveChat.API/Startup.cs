using System.Text;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Cors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using NSwag;
using NSwag.AspNetCore;
using NSwag.SwaggerGeneration.Processors.Security;

using LiveChat.API.Helpers;
using LiveChat.Data;
using LiveChat.Services;
using LiveChat.API.Hubs;
using System.Reflection;
using LiveChat.Services.ImageProcessing;

namespace LiveChat.API
{
    public class Startup
    {
        private const string DbConnectionStringName = "LivaChatDatabase";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Configure strongly typed settings objects
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            // DB Context
            services.AddDbContext<LiveChatDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString(DbConnectionStringName)),
                ServiceLifetime.Scoped);

            // CORS
            services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .AllowCredentials();
            }));

            // MVC
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                });

            // SignalR
            services.AddSignalR();

            // Configure jwt authentication
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            // Services
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IChatService, ChatService>();
            services.AddScoped<IImageWriter, ImageWriter>();
            services.AddScoped<IProfileService, ProfileService>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("MyPolicy");

            app.UseStaticFiles();

            app.UseSwaggerUi(
                typeof(Startup).GetTypeInfo().Assembly,
                settings =>
                {
                    settings.PostProcess = document =>
                    {
                        document.Info.Version = "v0.1";
                        document.Info.Title = "Liva Chat API";
                        document.Info.Description = "ASP.NET Core web API for Live Chat application";
                        document.Info.Contact = new SwaggerContact
                        {
                            Name = "Nikola Totkov",
                            Email = string.Empty,
                            Url = string.Empty
                        };
                        document.Info.License = new SwaggerLicense
                        {
                            Name = "Apache License 2.0",
                            Url = "https://www.apache.org/licenses/LICENSE-2.0"
                        };
                    };
                    settings.GeneratorSettings.OperationProcessors.Add(new OperationSecurityScopeProcessor("apiKey"));
                    settings.GeneratorSettings.DocumentProcessors.Add(new SecurityDefinitionAppender("apiKey", new SwaggerSecurityScheme()
                    {
                        Type = SwaggerSecuritySchemeType.ApiKey,
                        Name = "Authorization",
                        In = SwaggerSecurityApiKeyLocation.Header,
                        Description = "Bearer token"
                    }));
                });

            app.UseAuthentication();

            app.UseMvc();

            app.UseSignalR(route =>
            {
                route.MapHub<NotificationHub>("/notifications");
            });
            
        }
    }
}

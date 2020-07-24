using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using user_api.Services;

namespace user_api
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

            services.AddHttpClient("BackendApi").ConfigurePrimaryHttpMessageHandler(() =>
            {
                var handler = new HttpClientHandler();
                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                //if (environment == Environments.Development)
                //{
                //    handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
                //}


                handler.ServerCertificateCustomValidationCallback += (message, cert, chain, errors) => { return true; };
                return handler;


            });
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
            });

            services.AddControllersWithViews();

            Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;


            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;

            })
            .AddIdentityServerAuthentication(options =>
            {
                options.Authority = Configuration["Authority"];
                options.ApiSecret = "secret";
                options.ApiName = "USER-API";
                options.ApiName = "AUTH-SERVER";
                options.RequireHttpsMetadata = bool.Parse(Configuration["RequireHttpsMetadata"]);
            });


            services.AddAuthorization(options =>
            {
                options.AddPolicy("myPolicy", builder =>
                {
                    // require scope1
                    builder.RequireScope("USER-API");
                    builder.RequireScope("AUTH-SERVER");
                });
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "USER API", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Implicit = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri(Configuration["SwaggerAuthorityUrl"] + "/connect/authorize"),
                            Scopes = new Dictionary<string, string> { { "USER-API", "USER API Resources" }, { "AUTH-SERVER", "Auth Server API Resources" } }
                        },
                    },
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                        },
                        new List<string>{ "USER-API", "AUTH-SERVER" }
                    }
                });
            });

            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            //Declare DI containers
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IUserApiClient, UserApiClient>();
            services.AddTransient<IRoleApiClient, RoleApiClient>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSession();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(corsPolicyBuilder =>
            corsPolicyBuilder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
            );
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.OAuthClientId("swagger-user-api");
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "USER API V1");
            });
        }
    }
}

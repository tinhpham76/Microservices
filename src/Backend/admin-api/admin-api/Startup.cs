using admin_api.Data;
using admin_api.Services;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace admin_api
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
            //1. Setup entity framework
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Transient);

            // Config http client
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

            services.AddControllers();

            Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;

            })
            .AddIdentityServerAuthentication(options =>
            {
                options.Authority = Configuration["Authority"];
                options.ApiSecret = "secret";
                options.ApiName = "ADMIN-API";
                options.ApiName = "AUTH-SERVER";

            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("myPolicy", builder =>
                {
                    // require scope1
                    builder.RequireScope("ADMIN-API");
                    builder.RequireScope("AUTH-SERVER");
                });
            });

            // Config swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Admin API", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Implicit = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri(Configuration["SwaggerAuthorityUrl"] + "/connect/authorize"),
                            Scopes = new Dictionary<string, string> { { "ADMIN-API", "Admin API Resources" }, { "AUTH-SERVER", "Auth Server API Resources" } }
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
                        new List<string>{ "ADMIN-API", "AUTH-SERVER" }
                    }
                });
            });

            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            //Declare DI containers
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IClientApiClient, ClientApiClient>();
            services.AddTransient<IApiResourceApiClient, ApiResourceApiClient>();
            services.AddTransient<IIdentityResourceApiClient, IdentityResourceApiClient>();
            services.AddTransient<IApiScopeApiClient, ApiScopeApiClient>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseCors(corsPolicyBuilder =>
                corsPolicyBuilder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
            );

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.OAuthClientId("swagger-admin-api");
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ADMIN API V1");
            });
        }
    }
}

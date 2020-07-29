using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Serilog;

namespace gateway_api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            new WebHostBuilder()
            .UseKestrel()
            .UseContentRoot(Directory.GetCurrentDirectory())
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config
                    .SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
                    .AddJsonFile("appsettings.json", true, true)
                    .AddJsonFile(Path.Combine("configuration", "configuration.json"))
                    .AddEnvironmentVariables();
            })
            .ConfigureServices(services => {
                services.AddCors(options =>
                {
                    options.AddPolicy("CorsPolicy",
                        builder => builder.AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader());
                });

                services.AddOcelot();
                services.AddHttpContextAccessor();
            })
            .ConfigureLogging((hostingContext, logging) =>
            {
                Log.Logger = new LoggerConfiguration()
                        .Enrich.FromLogContext()
                        .WriteTo.Console()
                        .CreateLogger();
            })
            .UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration
                .ReadFrom.Configuration(hostingContext.Configuration))
            .UseIISIntegration()
            .Configure(app =>
            {
                app.UseCors("CorsPolicy");
                app.UseOcelot().Wait();
                
            })
            .Build()
            .Run();
        }
    }
}

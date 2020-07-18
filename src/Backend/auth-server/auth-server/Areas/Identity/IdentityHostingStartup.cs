using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(auth_server.Areas.Identity.IdentityHostingStartup))]
namespace auth_server.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
            });
        }
    }
}
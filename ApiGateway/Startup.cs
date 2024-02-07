using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Ocelot.Middleware;
using Ocelot.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.Net.Http.Headers;

namespace ApiGateway
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Register your services here
        }

        public void Configure(IApplicationBuilder app)
        {
            // Register your middleware here
            //app.UseMiddleware<MyCustomMiddleware>();
            //app.UseOcelot(new ApigeeCustomMiddleware()).Wait();

            Console.WriteLine("Custom Middleware: Request received.");     
            
        }

    }
}

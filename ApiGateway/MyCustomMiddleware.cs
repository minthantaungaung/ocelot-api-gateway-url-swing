using Microsoft.Extensions.Logging;
using Ocelot.Authorization;
using Ocelot.Logging;
using Ocelot.Middleware;
using static System.Net.Mime.MediaTypeNames;

namespace ApiGateway
{
    
    public class MyCustomMiddleware : OcelotMiddleware
    {
        private readonly IOcelotLogger _logger;
        private readonly RequestDelegate _next;
        public MyCustomMiddleware(RequestDelegate next, IOcelotLoggerFactory loggerFactory)
                : base(loggerFactory.CreateLogger<MyCustomMiddleware>())
        {
            _next = next;            
            _logger = loggerFactory.CreateLogger<MyCustomMiddleware>();
        }

        public async Task Invoke(HttpContext context)
        {
            Console.WriteLine("Custom Middleware: Request received.");
            // Your custom middleware logic goes here.
            // You can modify the context if needed.

            // Call the next middleware in the pipeline.
            await _next.Invoke(context);
        }
    }
}

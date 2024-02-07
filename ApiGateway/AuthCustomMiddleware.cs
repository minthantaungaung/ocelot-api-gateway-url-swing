using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Ocelot.Authorization;
using Ocelot.Authorization.Middleware;
using Ocelot.Logging;
using Ocelot.Middleware;
using Ocelot.Responses;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System.Xml.Linq;

namespace ApiGateway 
{
    public class AuthCustomMiddleware : Ocelot.Middleware.OcelotMiddleware
    {
        private readonly IOcelotLogger _logger;
        private readonly RequestDelegate _next;
        private readonly IClaimsAuthorizer _claimsAuthorizer;
        private readonly IScopesAuthorizer _scopesAuthorizer;

        private readonly ILogger<AuthCustomMiddleware> _MSlogger;

        #region Text Log
        internal void writeLog(string strValue)
        {
            try
            {
                //Logfile
                string path = "D:\\Logs\\OcelotApiGateway\\Track.txt";
                StreamWriter sw;
                if (!File.Exists(path))
                { sw = File.CreateText(path); }
                else
                { sw = File.AppendText(path); }

                LogWrite(strValue, sw);

                sw.Flush();
                sw.Close();
            }
            catch (Exception ex)
            {

            }
        }

        private static void LogWrite(string logMessage, StreamWriter w)
        {
            w.WriteLine("{0}", logMessage);
            w.WriteLine("----------------------------------------");
        }

        #endregion

        public AuthCustomMiddleware(RequestDelegate next,
                IClaimsAuthorizer claimsAuthorizer,
                IScopesAuthorizer scopesAuthorizer,
                IOcelotLoggerFactory loggerFactory,
                ILogger<AuthCustomMiddleware> mSlogger)
                : base(loggerFactory.CreateLogger<AuthorizationMiddleware>())
        {
            _next = next;
            _claimsAuthorizer = claimsAuthorizer;
            _scopesAuthorizer = scopesAuthorizer;
            _MSlogger = mSlogger;
            _logger = loggerFactory.CreateLogger<AuthorizationMiddleware>();
        }
        public async Task Invoke(HttpContext context)
        {
            Console.WriteLine("Custom Middleware: Request received.");
            writeLog("MS Custom middleware is being executed." + System.DateTime.Now);
            //_MSlogger.LogInformation("MS Custom middleware is being executed.");
            // Log a message to indicate the middleware is being executed
            _logger.LogInformation("Custom middleware is being executed.");

            // Check if the request has an Authorization header with a bearer token
            if (context.Request.Headers.ContainsKey("Authorization"))
            {
                var authorizationHeader = context.Request.Headers["Authorization"].ToString();

                // Check if it's a bearer token
                if (authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    var token = authorizationHeader.Substring("Bearer ".Length).Trim();

                    // Now, you have the bearer token. You can perform token validation here.
                    // You can use a library like IdentityServer4 or similar to validate the token.
                    // If the token is valid, proceed with the request; otherwise, return an unauthorized response.

                    // Replace the existing token with your new token
                    string tempToken = "eyJraWQiOiIxIiwidHlwIjoiSldUIiwiYWxnIjoiUlMyNTYifQ.eyJzdWIiOiJhcGlyIiwiYXVkIjoiYXBpcl9jbGllbnQiLCJ1c2VyX25hbWUiOiJEUUxPQyIsImlzcyI6InVybjpcL1wvYXBpZ2VlLWVkZ2UtYXV0aCIsImV4cCI6MTY5NzUzNjgzMCwiaWF0IjoxNjk3NTI2MDMwLCJjbGllbnRfaWQiOiJQMmpTZm1UeDFOeFZMNEhqbEdPejRBczBQQksyTkR6cyIsImp0aSI6IjJlMWQ4MzQ3LWM2MDUtNGZmOS04MjQ2LTY5YzdlODM0YWE4YyJ9.I41ZnUxWcTH_lFkBJjtavyOnrbsvUKmx4nbvD8VAccF-02wjYOdSP_Fc-7eU0TIKo6U4dRVedmduh4UQow81UcUFRSEgjxzneeVgZ3C_vVY2isI3HMnul6oJSHaF4_KCJq7GtF6T7FzWQ7Pt87UQgdt3VdfdH-dzRPlF6rA7trrThc0IhVrDvl84mPE6vxTt42qRUwU1yHsJXbDL3uY6z75f_T6IoXFKLrEZwRCwwbu5Lzijj2Ss-1J74RixVjqYzzf_OoSDp2jf8ZJDj5zm2BojvJhrimxcNKga3hEJcKEsoRFyk8J9T_n-OL-YLTpLr4xMIgNq2uu31ZYoMYeiWQ";
                    var newToken = "Bearer " + tempToken; // Replace with your new token
                    //context.Request.Headers["Authorization"] = newToken;

                    var newAuthHeader = new AuthenticationHeaderValue("Bearer", newToken);

                    // Set the new Authorization header
                    context.Request.Headers["Authorization"] = new StringValues(newAuthHeader.ToString());

                }
            }

            await _next.Invoke(context);
        }
    }

    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthCustomMiddleware>(); // Replace with the actual name of your custom middleware class
        }
    }

    
}


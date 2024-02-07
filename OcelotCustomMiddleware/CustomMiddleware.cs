using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Ocelot.Authorization;
using Ocelot.Authorization.Middleware;
using Ocelot.Logging;
using Ocelot.Middleware;
using Ocelot.Responses;
using static System.Net.Mime.MediaTypeNames;

public class CustomMiddleware : Ocelot.Middleware.OcelotMiddleware
{
    private readonly IOcelotLogger _logger;
    private readonly RequestDelegate _next;
    private readonly IClaimsAuthorizer _claimsAuthorizer;
    private readonly IScopesAuthorizer _scopesAuthorizer;
    public CustomMiddleware(RequestDelegate next,
            IClaimsAuthorizer claimsAuthorizer,
            IScopesAuthorizer scopesAuthorizer,
            IOcelotLoggerFactory loggerFactory)
            : base(loggerFactory.CreateLogger<AuthorizationMiddleware>())
    {
        _next = next;
        _claimsAuthorizer = claimsAuthorizer;
        _scopesAuthorizer = scopesAuthorizer;
    }
    public async Task Invoke(HttpContext context)
    {

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
                var newToken = "Bearer "+ tempToken; // Replace with your new token
                context.Request.Headers["Authorization"] = newToken;


            }
        }



        await _next.Invoke(context);
    }  
}

/* Working CustomMiddleware */
/*
 public class CustomMiddleware : Ocelot.Middleware.OcelotMiddleware
{
    private readonly IOcelotLogger _logger;
    private readonly RequestDelegate _next;
    private readonly IClaimsAuthorizer _claimsAuthorizer;
    private readonly IScopesAuthorizer _scopesAuthorizer;


    public CustomMiddleware(RequestDelegate next,
            IClaimsAuthorizer claimsAuthorizer,
            IScopesAuthorizer scopesAuthorizer,
            IOcelotLoggerFactory loggerFactory)
            : base(loggerFactory.CreateLogger<AuthorizationMiddleware>())
    {
        _next = next;
        _claimsAuthorizer = claimsAuthorizer;
        _scopesAuthorizer = scopesAuthorizer;
    }

    public async Task Invoke(HttpContext context)
    {
        await _next.Invoke(context);
    }  
}

*/



//public async Task Invoke(DownstreamResponse response)
//{
//    // Your custom middleware logic goes here.
//    // You can modify the context if needed.

//    // Call the next middleware in the pipeline.
//    await Next.Invoke(context);
//}


/*
public class CustomMiddleware : OcelotMiddleware
{
    public CustomMiddleware(RequestDelegate next, IOcelotLoggerFactory loggerFactory)
        : base(loggerFactory.CreateLogger<CustomMiddleware>())
    {
    }

    public async Task Invoke(DownstreamResponse response)
    {
        // Your custom middleware logic goes here.
        // You can modify the context if needed.

        // Call the next middleware in the pipeline.
        await responder.SetResponseOnHttpContext(HttpContext, response);
    }
}
*/


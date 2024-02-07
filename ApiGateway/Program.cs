using ApiGateway;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

var startup = new Startup(builder.Configuration);

builder.Configuration.SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)    
    .AddEnvironmentVariables();

builder.Services.AddOcelot(builder.Configuration);

startup.ConfigureServices(builder.Services);

// Remove this line as it's not needed anymore.
// builder.Services.AddHostedService<Startup>();

var app = builder.Build();

startup.Configure(app);

//app.UseCors();

app.UseRouting(); // Add this line to enable routing.



var configuration = new OcelotPipelineConfiguration
{
    PreAuthenticationMiddleware = async (ctx, next) =>
    {

        if (ctx.Request.Headers.ContainsKey("Authorization"))
        {
            var authorizationHeader = ctx.Request.Headers["Authorization"].ToString();

            // Check if it's a bearer token
            if (authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                var token = authorizationHeader.Substring("Bearer ".Length).Trim();

                // Now, you have the bearer token. You can perform token validation here.
                // You can use a library like IdentityServer4 or similar to validate the token.
                // If the token is valid, proceed with the request; otherwise, return an unauthorized response.

                // Replace the existing token with your new token
                string tempToken = "eyJraWQiOiIxIiwidHlwIjoiSldUIiwiYWxnIjoiUlMyNTYifQ.eyJzdWIiOiJhcGlyIiwiYXVkIjoiYXBpcl9jbGllbnQiLCJ1c2VyX25hbWUiOiJEUUxPQyIsImlzcyI6InVybjpcL1wvYXBpZ2VlLWVkZ2UtYXV0aCIsImV4cCI6MTY5NzY1NjY3MiwiaWF0IjoxNjk3NjQ1ODcyLCJjbGllbnRfaWQiOiJQMmpTZm1UeDFOeFZMNEhqbEdPejRBczBQQksyTkR6cyIsImp0aSI6Ijk0MTlhMTgzLTc1MGEtNGQ3Ni1hYmFiLTdlNWJmOWYzOTRjMCJ9.Tiu4VSQAI5LIyOkKMOZD_6TggQfExv5tywNv_xEmJt_S7JQv_227NjvoVA06hmPRMo0yNYQpPZHLKY0ojDeP1hDmwv905sHG6eEtQu2lPP16EI-rt7FKQqzKybJchuPBczOhXTqQyiCCBaNIvfosucSrNVfU7kFeMk8biUYigamIAEx3S77qdKePVguOCeiSq_kEtJEiW4JbrtL5go3uK0OsfPUIAEwOAefVBA78mV388cYPBn6JuyNzA3JVTMyci14EGoYs17hfNPsbdpqFeyVzGFdrJj9TDDWDxriYvgeoCAMvKwPj4tZl_G8lZ2PYju9tO37F-7LG3jSiqheMog";
                //var newToken = "Bearer " + tempToken; // Replace with your new token
                                                      //context.Request.Headers["Authorization"] = newToken;

                var newAuthHeader = new AuthenticationHeaderValue("Bearer", tempToken);

                //ctx.Request.Headers["Authorization"] = 

                // Set the new Authorization header
                ctx.Request.Headers["Authorization"] = new StringValues(newAuthHeader.ToString());

            }
        }

        await next.Invoke();
    }
};

// Register Ocelot middleware and your custom middleware.
app.UseOcelot(configuration).Wait();
//app.UseCustomMiddleware();

app.UseAuthentication();

app.UseAuthorization();

app.Run();

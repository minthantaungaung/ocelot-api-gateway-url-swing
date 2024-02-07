using JwtAuthenticationManager.Models;
using JwtAuthenticationManager;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AuthenticationWebApi;

namespace AuthenticationWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApigeeTokenController : ControllerBase
    {
        private readonly AuthHelper _jwtApigeeTokenHandler;
        private X509Certificate2 certificate;
        //private string certificatePath = "D:\\KBZ Workspace\\React\\Ocelot Api Gateway\\OcelotApiGateway\\OcelotApiGateway\\AuthenticationWebApi\\Certs\\privatekey.cer";

        public ApigeeTokenController(AuthHelper jwtApigeeTokenHandler)
        {
            _jwtApigeeTokenHandler = jwtApigeeTokenHandler;
            

        }

        //public ActionResult<AuthenticationResponse?> Authenticate([FromBody] AuthenticationRequest authenticationRequest)
        [HttpPost]
        public ActionResult<string?> Authenticate([FromBody] AuthenticationRequest authenticationRequest)
        {
            
            var privateKeyPEM = System.IO.File.ReadAllText("Certs/privatekey.pem");
            //this.certificate = new X509Certificate2(certificatePath);
            RSA privateKey = _jwtApigeeTokenHandler.LoadPrivateKeyFromPEM();
            string jwtToken = _jwtApigeeTokenHandler.CreateApgJwtToken(privateKey);

            //var authenticationResponse = _jwtTokenHandler.GenerateApigeeCompatibleJwtToken(authenticationRequest, certificate);
            //if (authenticationResponse == null) return Unauthorized();
            //return authenticationResponse;

            return jwtToken;
        }

        

        

    }
}

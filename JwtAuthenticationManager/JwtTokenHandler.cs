using JwtAuthenticationManager.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace JwtAuthenticationManager
{
    public class JwtTokenHandler
    {
        public const string JWT_SECURITY_KEY = "yPkCqn4kSWLtaJwXvN2jGzpQRyTZ3gdXkt7FeBJP";
        private const int JWT_TOKEN_VALIDITY_MINS = 20;
        private readonly List<UserAccount> _userAccountList;

        //Apigee
        private const int APIGEE_JWT_TOKEN_VALIDITY_MINS = 20;
        private readonly List<TokenUserAccount> _userTokenAccountList;
        private readonly X509Certificate2 certificate;
        private readonly string certificatePath = "./Certs/privatekey.cer";

        public JwtTokenHandler()
        {
            _userAccountList = new List<UserAccount>
            {
                new UserAccount{ UserName = "admin", Password = "admin123", Role = "Administrator" },
                new UserAccount{ UserName = "user01", Password = "user01", Role = "User" }
            };

            _userTokenAccountList = new List<TokenUserAccount>
            {
                new TokenUserAccount { UserName = "admin", Password = "admin123", Role = "Administrator" },
                new TokenUserAccount { UserName = "user01", Password = "user01", Role = "User" }
            };
        }

        ////Apigee        
        //public JwtTokenHandler(string certificatePath)
        //{
        //    _userTokenAccountList = new List<TokenUserAccount>
        //    {
        //        new TokenUserAccount { UserName = "admin", Password = "admin123", Role = "Administrator" },
        //        new TokenUserAccount { UserName = "user01", Password = "user01", Role = "User" }
        //    };            

        //    X509Certificate2 certificate = new X509Certificate2(certificatePath);
        //    this.certificate = certificate;
        //}

        public AuthenticationResponse? GenerateJwtToken(AuthenticationRequest authenticationRequest)
        {
            if (string.IsNullOrWhiteSpace(authenticationRequest.UserName) || string.IsNullOrWhiteSpace(authenticationRequest.Password))
                return null;

            /* Validation */
            var userAccount = _userAccountList.Where(x => x.UserName == authenticationRequest.UserName && x.Password == authenticationRequest.Password).FirstOrDefault();
            if (userAccount == null) return null;

            var tokenExpiryTimeStamp = DateTime.Now.AddMinutes(JWT_TOKEN_VALIDITY_MINS);
            var tokenKey = Encoding.ASCII.GetBytes(JWT_SECURITY_KEY);
            var claimsIdentity = new ClaimsIdentity(new List<Claim>
            { 
                new Claim(JwtRegisteredClaimNames.Name,authenticationRequest.UserName),
                new Claim("Role",userAccount.Role)
            
            });

            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(tokenKey),
                SecurityAlgorithms.HmacSha256Signature);

            var securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Expires = tokenExpiryTimeStamp,
                SigningCredentials = signingCredentials
            };

            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var securityToken = jwtSecurityTokenHandler.CreateToken(securityTokenDescriptor);
            var token = jwtSecurityTokenHandler.WriteToken(securityToken);

            return new AuthenticationResponse
            {
                UserName = userAccount.UserName,
                ExpiresIn = (int)tokenExpiryTimeStamp.Subtract(DateTime.Now).TotalSeconds,
                JwtToken = token
            };

        }

        //Apigee Token Generate 
        

        public AuthenticationResponse? GenerateApigeeCompatibleJwtToken(AuthenticationRequest request, X509Certificate2 certificate)
        {
            if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Password))
                return null;

            /* Validation */
            var userAccount = _userTokenAccountList.FirstOrDefault(x => x.UserName == request.UserName && x.Password == request.Password);
            if (userAccount == null)
                return null;

            var tokenExpiryTimeStamp = DateTime.Now.AddMinutes(JWT_TOKEN_VALIDITY_MINS);

            var rsaSecurityKey = new RsaSecurityKey(certificate.GetRSAPrivateKey());

            DateTime dateTime = DateTime.Now; // or any other DateTime
            long unixEpochExpireTime = ((DateTimeOffset)dateTime.AddMinutes(60)).ToUnixTimeSeconds();
            long unixEpochTime = ((DateTimeOffset)dateTime).ToUnixTimeSeconds();

            var claimsIdentity = new ClaimsIdentity(new List<Claim>
                {
                    //new Claim(JwtRegisteredClaimNames.Name, request.UserName),
                    new Claim("sub","apir"),
                    new Claim("aud","apir_client"),
                    new Claim("user_name","DQLOC"),
                    new Claim("iss","urn://apigee-edge-auth"),
                    new Claim("exp",unixEpochExpireTime.ToString()),
                    new Claim("iat",unixEpochTime.ToString()),
                    new Claim("client_id","P2jSfmTx1NxVL4HjlGOz4As0PBK2NDzs"),
                    new Claim("jti", "dfbe610e-6ae2-4b87-9d05-3cd0d3448b63"),
                    new Claim("Role", userAccount.Role)
                });

            var signingCredentials = new SigningCredentials(rsaSecurityKey, SecurityAlgorithms.RsaSha256);

            var securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Expires = tokenExpiryTimeStamp,
                SigningCredentials = signingCredentials
            };

            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var securityToken = jwtSecurityTokenHandler.CreateToken(securityTokenDescriptor);
            var token = jwtSecurityTokenHandler.WriteToken(securityToken);

            return new AuthenticationResponse
            {
                UserName = userAccount.UserName,
                ExpiresIn = (int)tokenExpiryTimeStamp.Subtract(DateTime.Now).TotalSeconds,
                JwtToken = token
            };
        }
        






    }
}

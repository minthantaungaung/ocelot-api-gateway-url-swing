using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace AuthenticationWebApi
{
    public class AuthHelper
    {
        protected string privateKeyPEM;
        public AuthHelper() 
        {
            privateKeyPEM = System.IO.File.ReadAllText("Certs/privatekey.pem");
        }            

        public RSA LoadPrivateKeyFromPEM()
        {
            RSA privateKey = RSA.Create();

            if (privateKeyPEM.Contains("BEGIN RSA PRIVATE KEY"))
            {
                // The key is in the older RSA format
                var privateKeyBytes = Convert.FromBase64String(privateKeyPEM
                    .Replace("-----BEGIN RSA PRIVATE KEY-----", "")
                    .Replace("-----END RSA PRIVATE KEY-----", ""));
                privateKey.ImportRSAPrivateKey(privateKeyBytes, out _);
            }
            //else
            //{
            //    // The key is in the newer PKCS#8 format
            //    var privateKeyBytes = Convert.FromBase64String(privateKeyPEM
            //        .Replace("-----BEGIN PRIVATE KEY-----", "")
            //        .Replace("-----END PRIVATE KEY-----", ""));
            //    privateKey.ImportFromPem(privateKeyBytes);
            //}

            return privateKey;
        }

        public string CreateApgJwtToken()
        {
            RSA privateKey = LoadPrivateKeyFromPEM();
            using var rsa = privateKey;
            var rsaSecurityKey = new RsaSecurityKey(rsa)
            {
                KeyId = "1"
            };

            var signingCredentials = new SigningCredentials(rsaSecurityKey, SecurityAlgorithms.RsaSha256);

            DateTime dateTime = DateTime.Now; // or any other DateTime
            long unixEpochExpireTime = ((DateTimeOffset)dateTime.AddMinutes(60)).ToUnixTimeSeconds();
            long unixEpochTime = ((DateTimeOffset)dateTime).ToUnixTimeSeconds();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    //new Claim(ClaimTypes.Name, "username"),
                    //new Claim(ClaimTypes.Role, "user")

                    new Claim("sub","apir"),
                    new Claim("aud","apir_client"),
                    new Claim("user_name","DQLOC"),
                    new Claim("role","maker"),
                    new Claim("iss","urn://apigee-edge-auth"),
                    new Claim("exp",unixEpochExpireTime.ToString()),
                    new Claim("iat",unixEpochTime.ToString()),
                    new Claim("client_id","P2jSfmTx1NxVL4HjlGOz4As0PBK2NDzs"),
                    new Claim("jti", "dfbe610e-6ae2-4b87-9d05-3cd0d3448b63")
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = signingCredentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return tokenString;
        }


    }
}

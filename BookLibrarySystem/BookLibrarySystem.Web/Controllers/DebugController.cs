using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace BookLibrarySystem.Web.Controllers
{
    [ApiController]
    [Route("debug")]
    public class DebugController : ControllerBase
    {
        private readonly IKeyMaterialService _keyMaterialService;

        public DebugController(IKeyMaterialService keyMaterialService)
        {
            _keyMaterialService = keyMaterialService;
        }

        [HttpGet("signing-key")]
        public async Task<IActionResult> GetSigningKey()
        {
            // Retrieve the signing key material (for tests only)
            var key = await _keyMaterialService.GetSigningCredentialsAsync();
            if (key?.Key is null) return NotFound("No signing key found.");

            // Convert the key to a format that can be reused
            // Handle X509SecurityKey
            if (key.Key is Microsoft.IdentityModel.Tokens.X509SecurityKey x509SecurityKey)
            {
                var certificate = x509SecurityKey.Certificate;

                // Check if the certificate contains a private key
                if (!certificate.HasPrivateKey)
                    return BadRequest("The certificate does not contain a private key.");

                // Extract the private key
                using var rsa = certificate.GetRSAPrivateKey();
                if (rsa == null)
                    return BadRequest("Unable to extract RSA private key.");

                // Export the private key as PEM
                //var privateKeyPem = ExportPrivateKeyToPem(rsa);
                //return Ok(privateKeyPem);

                // Create signing credentials
                var rsaSecurityKey = new RsaSecurityKey(rsa)
                {
                    KeyId = x509SecurityKey.KeyId // Set the KeyId to match the certificate's KeyId
                };
                var signingCredentials = new SigningCredentials(rsaSecurityKey, SecurityAlgorithms.RsaSha256);

                // Create custom headers
                var header = new JwtHeader(signingCredentials)
                {
                    //Typ = "at+jwt"
                    //{ "kid", x509SecurityKey.KeyId }, // Explicitly add 'kid'
                    //{ "x5t", "TJjRIg36UQbyRW5w5Lunw_L0I3M" }, // Add X.509 thumbprint
                    //{ "typ", "at+jwt" } // Override the default 'JWT' type with 'at+jwt'
                };
                header["kid"] = x509SecurityKey.KeyId;
                header["typ"] = "at+jwt";
                header["x5t"] = Base64UrlEncoder.Encode(certificate.GetCertHash());// "TJjRIg36UQbyRW5w5Lunw_L0I3M";

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Claims = new Dictionary<string, object>
                {
                    { "iss", "https://localhost:44490" },
                    { "nbf", new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString()},
                    { "iat", new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString() },
                    { "exp", new DateTimeOffset(DateTime.UtcNow.AddHours(1)).ToUnixTimeSeconds().ToString() },
                    { "aud", "BookLibrarySystem.WebAPI" },
                    { "scope", new List<string>{"BookLibrarySystem.WebAPI", "openid", "profile" } },
                    { "amr", new List<string>{"external" } },
                    { "client_id", "BookLibrarySystem" },
                    { "sub", "7b125073-72e7-4f69-8ad1-05eae69a899e" },
                    { "auth_time", new DateTimeOffset(DateTime.UtcNow.AddSeconds(-1)).ToUnixTimeSeconds().ToString() },
                    { "idp", "Google"},
                    { "sid", "7E6390CF00CC47A30D5B2779D91F3304" },
                    { "jti", Guid.NewGuid().ToString() }
                },
                    Expires = DateTime.UtcNow.AddMinutes(30),
                    SigningCredentials = signingCredentials                   
                };

                var payload = new JwtPayload
                {
                    { "iss", "https://localhost:44490" },
                    { "nbf", new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString()},
                    { "iat", new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString() },
                    { "exp", new DateTimeOffset(DateTime.UtcNow.AddHours(1)).ToUnixTimeSeconds().ToString() },
                    { "aud", "BookLibrarySystem.WebAPI" },
                    { "scope", new List<string>{"BookLibrarySystem.WebAPI", "openid", "profile" } },
                    { "amr", new List<string>{"external"} },
                    { "client_id", "BookLibrarySystem" },
                    { "sub", "7b125073-72e7-4f69-8ad1-05eae69a899e" },
                    { "auth_time", new DateTimeOffset(DateTime.UtcNow.AddSeconds(-1)).ToUnixTimeSeconds().ToString() },
                    { "idp", "Google"},
                    { "sid", "7E6390CF00CC47A30D5B2779D91F3304" },
                    { "jti", Guid.NewGuid().ToString() }
                };

                //var token = tokenHandler.CreateToken(tokenDescriptor);
                // Create token
                var token = new JwtSecurityToken(header, payload);
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwt = tokenHandler.WriteToken(token);
                return Ok(jwt);
            }

            return BadRequest("Unsupported signing key type.");
        }

        private string ExportPrivateKeyToPem(RSA rsaCng)
        {
            // Export the RSA key as parameters (requires an exportable key)
            var rsaParameters = rsaCng.ExportParameters(true);

            using var stringWriter = new StringWriter();
            var pemWriter = new Org.BouncyCastle.OpenSsl.PemWriter(stringWriter);

            // Convert RSAParameters to BouncyCastle key
            var keyPair = Org.BouncyCastle.Security.DotNetUtilities.GetRsaKeyPair(rsaParameters);
            pemWriter.WriteObject(keyPair.Private);
            pemWriter.Writer.Flush();

            return stringWriter.ToString();
        }
    }
}

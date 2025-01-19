using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookLibrarySystem.Web.Providers
{
    public class CustomAuthenticationSchemeProvider : AuthenticationSchemeProvider
    {
        private readonly IConfiguration _configuration;

        public CustomAuthenticationSchemeProvider(
       IOptions<AuthenticationOptions> options,
       IConfiguration configuration)
       : base(options)
        {
            _configuration = configuration;
        }

        public async override Task<AuthenticationScheme?> GetDefaultAuthenticateSchemeAsync()
        {
            // Dynamically determine the default authentication scheme
            var authMode = _configuration["Authentication:AuthMode"] ?? "Google";
            var schemes = await GetAllSchemesAsync();

            if (authMode == "Google")
            {
                return schemes.FirstOrDefault(s => s.Name == "Google");
            }

            if (authMode == "GuidKey")
            {
                return schemes.FirstOrDefault(s => s.Name == "GuidKeyScheme");
            }

            return default(AuthenticationScheme?); // No default scheme if not matched
        }

        public override Task<AuthenticationScheme?> GetDefaultChallengeSchemeAsync()
        {
            // Challenge scheme for redirection during unauthorized requests
            return GetDefaultAuthenticateSchemeAsync();
        }
    }
}

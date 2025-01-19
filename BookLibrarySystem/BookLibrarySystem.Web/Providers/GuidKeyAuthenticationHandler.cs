using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Data;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace BookLibrarySystem.Web.Providers
{
    public class GuidKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private const string ApiKeyHeaderName = "X-Api-Key";
        private readonly string _validApiKey;

        public GuidKeyAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IConfiguration configuration)
            : base(options, logger, encoder, clock)
        {
            _validApiKey = configuration["Authentication:GuidKey"] ?? throw new InvalidOperationException("GUID Key not set");
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue(ApiKeyHeaderName, out var apiKeyValue))
            {
                return Task.FromResult(AuthenticateResult.Fail("API Key is missing."));
            }

            if (!Guid.TryParse(apiKeyValue, out var providedGuid) || providedGuid.ToString() != _validApiKey)
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid API Key."));
            }

            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, "GuidKeyUser"),
                new Claim(ClaimTypes.Role, value: "Administrator"),
                new Claim(ClaimTypes.Role, value: "NormalUser")
            };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}

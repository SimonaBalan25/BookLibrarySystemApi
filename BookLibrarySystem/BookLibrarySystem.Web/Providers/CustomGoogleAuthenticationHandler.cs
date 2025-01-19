using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using System.Security.Claims;

namespace BookLibrarySystem.Web.Providers
{
    public class CustomGoogleAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public CustomGoogleAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claimsSet =(Context.User.Identity as ClaimsIdentity)?.Claims;
            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, "GoogleUser"),
                new Claim(ClaimTypes.Role, value: "Administrator"),
                new Claim(ClaimTypes.Role, value: "NormalUser")
            };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
    }

}

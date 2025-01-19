using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;

namespace BookLibrarySystem.Web.Providers
{
    public class DynamicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IOptionsMonitor<AuthenticationSettings> _appSettings;

        public DynamicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IOptionsMonitor<AuthenticationSettings> appSettings)
            : base(options, logger, encoder, clock)
        {
            _appSettings = appSettings;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var authMode = _appSettings.CurrentValue.AuthMode;

            if (authMode == "Google")
            {
                var result = await Context.AuthenticateAsync("GoogleCustom");
                if (!result.Succeeded)
                {
                    return AuthenticateResult.Fail("Failed to authenticate using Google.");
                }
                return result;
            }
            else if (authMode == "GuidKey")
            {
                var result = await Context.AuthenticateAsync("GuidKeyScheme");
                if (!result.Succeeded)
                {
                    return AuthenticateResult.Fail("Failed to authenticate using GUID Key.");
                }
                return result;
            }
            else
            {
                return AuthenticateResult.Fail("Unsupported authentication mode.");
            }
        }
    }

    public class AuthenticationSettings
    {
        public string AuthMode { get; set; }  // Google or GuidKey

        public GoogleSettings Google { get; set; }

        public string GuidKey { get; set; }
    }

    public class GoogleSettings
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}

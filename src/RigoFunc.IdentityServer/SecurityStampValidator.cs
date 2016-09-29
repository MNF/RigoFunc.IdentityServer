using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Builder;

namespace RigoFunc.IdentityServer {
    public class SecurityStampValidator<TUser> : ISecurityStampValidator where TUser : class {
        private readonly IdentityOptions _options;
        private readonly SignInManager<TUser> _signInManager;

        public SecurityStampValidator(IOptions<IdentityOptions> options, SignInManager<TUser> signInManager) {
            _options = options.Value;
            _signInManager = signInManager;
        }

        public async Task ValidateAsync(CookieValidatePrincipalContext context) {
            var currentUtc = DateTimeOffset.UtcNow;
            if (context.Options != null && context.Options.SystemClock != null) {
                currentUtc = context.Options.SystemClock.UtcNow;
            }
            var issuedUtc = context.Properties.IssuedUtc;

            // Only validate if enough time has elapsed
            var validate = (issuedUtc == null);
            if (issuedUtc != null) {
                var timeElapsed = currentUtc.Subtract(issuedUtc.Value);
                validate = timeElapsed > _options.SecurityStampValidationInterval;
            }
            if (validate) {
                var user = await _signInManager.ValidateSecurityStampAsync(context.Principal);
                if (user != null) {
                    // REVIEW: note we lost login authenticaiton method

                    // Brock Allen: this line have been deliberatly commented out (see comment above)
                    //context.ReplacePrincipal(await _signInManager.CreateUserPrincipalAsync(user));

                    // Brock Allen: we leave this line so a new cookie is issued with updated
                    // issused and expires values
                    context.ShouldRenew = true;
                }
                else {
                    context.RejectPrincipal();
                    await _signInManager.SignOutAsync();
                }
            }
        }
    }
}

using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace RigoFunc.IdentityServer {
    public class UserClaimsFactory<TUser, TRole> : UserClaimsPrincipalFactory<TUser, TRole>
        where TUser : class
        where TRole : class {
        public UserClaimsFactory(UserManager<TUser> userManager, RoleManager<TRole> roleManager, IOptions<IdentityOptions> optionsAccessor) : base(userManager, roleManager, optionsAccessor) {
        }

        public async override Task<ClaimsPrincipal> CreateAsync(TUser user) {
            var principal = await base.CreateAsync(user);
            var identity = principal.Identities.First();

            var userName = await UserManager.GetUserNameAsync(user);
            var userNameClaim = identity.FindFirst(claim => claim.Type == Options.ClaimsIdentity.UserNameClaimType && claim.Value == userName);
            if (userNameClaim != null) {
                identity.RemoveClaim(userNameClaim);
                identity.AddClaim(new Claim(JwtClaimTypes.PreferredUserName, userName));
            }

            if (!identity.HasClaim(x => x.Type == JwtClaimTypes.Name)) {
                identity.AddClaim(new Claim(JwtClaimTypes.Name, userName));
            }

            if (UserManager.SupportsUserEmail) {
                var email = await UserManager.GetEmailAsync(user);
                if (!string.IsNullOrWhiteSpace(email)) {
                    identity.AddClaims(new[]
                    {
                        new Claim(JwtClaimTypes.Email, email),
                        new Claim(JwtClaimTypes.EmailVerified,
                            await UserManager.IsEmailConfirmedAsync(user) ? "true" : "false", ClaimValueTypes.Boolean)
                    });
                }
            }

            if (UserManager.SupportsUserPhoneNumber) {
                var phoneNumber = await UserManager.GetPhoneNumberAsync(user);
                if (!string.IsNullOrWhiteSpace(phoneNumber)) {
                    identity.AddClaims(new[]
                    {
                        new Claim(JwtClaimTypes.PhoneNumber, phoneNumber),
                        new Claim(JwtClaimTypes.PhoneNumberVerified,
                            await UserManager.IsPhoneNumberConfirmedAsync(user) ? "true" : "false", ClaimValueTypes.Boolean)
                    });
                }
            }

            return principal;
        }
    }
}

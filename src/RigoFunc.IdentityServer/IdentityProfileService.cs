using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;

namespace RigoFunc.IdentityServer {
    /// <summary>
    /// The implementation of <see cref="IProfileService"/> allows IdentityServer to get identity user and profile store.
    /// </summary>
    /// <typeparam name="TUser">The type of the user.</typeparam>
    public class IdentityProfileService<TUser> : IProfileService
        where TUser : class {
        private readonly IUserClaimsPrincipalFactory<TUser> _claimsFactory;
        private readonly UserManager<TUser> _userManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityProfileService{TUser}"/> class.
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        /// <param name="claimsFactory">The claims factory.</param>
        public IdentityProfileService(UserManager<TUser> userManager,
            IUserClaimsPrincipalFactory<TUser> claimsFactory) {
            _userManager = userManager;
            _claimsFactory = claimsFactory;
        }

        /// <summary>
        /// This method is called whenever claims about the user are requested (e.g. during token creation or via the userinfo endpoint)
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public async Task GetProfileDataAsync(ProfileDataRequestContext context) {
            var sub = context.Subject.GetSubjectId();
            var user = await _userManager.FindByIdAsync(sub);
            var principal = await _claimsFactory.CreateAsync(user);

            var claims = new List<Claim>(principal.Claims);
            
            // Add phone & email
            if (_userManager.SupportsUserEmail) {
                var email = await _userManager.GetEmailAsync(user);
                if (!string.IsNullOrWhiteSpace(email)) {
                    claims.AddRange(new[]
                    {
                        new Claim(JwtClaimTypes.Email, email),
                        new Claim(JwtClaimTypes.EmailVerified,
                            await _userManager.IsEmailConfirmedAsync(user) ? "true" : "false", ClaimValueTypes.Boolean)
                    });
                }
            }

            if (_userManager.SupportsUserPhoneNumber) {
                var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
                if (!string.IsNullOrWhiteSpace(phoneNumber)) {
                    claims.AddRange(new[]
                    {
                        new Claim(JwtClaimTypes.PhoneNumber, phoneNumber),
                        new Claim(JwtClaimTypes.PhoneNumberVerified,
                            await _userManager.IsPhoneNumberConfirmedAsync(user) ? "true" : "false", ClaimValueTypes.Boolean)
                    });
                }
            }

            context.AddFilteredClaims(claims);
        }

        /// <summary>
        /// This method gets called whenever identity server needs to determine if the user is valid or active (e.g. if the user's account has been deactivated since they logged in).
        /// (e.g. during token issuance or validation).
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public async Task IsActiveAsync(IsActiveContext context) {
            var sub = context.Subject.GetSubjectId();
            var user = await _userManager.FindByIdAsync(sub);
            context.IsActive = user != null;
        }
    }
}

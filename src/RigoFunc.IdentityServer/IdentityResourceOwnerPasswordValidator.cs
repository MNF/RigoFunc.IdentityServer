using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using static IdentityModel.OidcConstants;

namespace RigoFunc.IdentityServer {
    /// <summary>
    /// Handles validation of resource owner password credentials
    /// </summary>
    /// <typeparam name="TUser">The type of the t user.</typeparam>
    public class IdentityResourceOwnerPasswordValidator<TUser> : IResourceOwnerPasswordValidator where TUser : class {
        private readonly UserManager<TUser> _userManager;
        private readonly SignInManager<TUser> _signInManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityResourceOwnerPasswordValidator{TUser}"/> class.
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        /// <param name="signInManager">The sign in manager.</param>
        public IdentityResourceOwnerPasswordValidator(UserManager<TUser> userManager, SignInManager<TUser> signInManager) {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        /// Validates the resource owner password credential
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>Task.</returns>
        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context) {
            var user = await _userManager.FindByNameAsync(context.UserName);
            if (user != null) {
                if (await _signInManager.CanSignInAsync(user)) {
                    if (_userManager.SupportsUserLockout && await _userManager.IsLockedOutAsync(user)) {
                        context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, $"The user: {context.UserName} is lockout");
                    }
                    else if (await _userManager.CheckPasswordAsync(user, context.Password)) {
                        if (_userManager.SupportsUserLockout) {
                            await _userManager.ResetAccessFailedCountAsync(user);
                        }
                        var sub = await _userManager.GetUserIdAsync(user);

                        context.Result = new GrantValidationResult(sub, AuthenticationMethods.Password);
                    }
                    else {
                        int code;
                        if (int.TryParse(context.Password, out code) && _userManager.SupportsUserPhoneNumber) {
                            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
                            if (await _userManager.VerifyChangePhoneNumberTokenAsync(user, context.Password, phoneNumber)) {
                                if (_userManager.SupportsUserLockout) {
                                    await _userManager.ResetAccessFailedCountAsync(user);
                                }
                                var sub = await _userManager.GetUserIdAsync(user);

                                context.Result = new GrantValidationResult(sub, AuthenticationMethods.Password);
                            }
                        }
                        else if (_userManager.SupportsUserLockout) {
                            await _userManager.AccessFailedAsync(user);
                        }
                    }
                }
            }
        }
    }
}

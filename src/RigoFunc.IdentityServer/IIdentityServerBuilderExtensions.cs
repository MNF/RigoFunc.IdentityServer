using IdentityModel;
using IdentityServer4.Configuration;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using RigoFunc.IdentityServer;

namespace Microsoft.Extensions.DependencyInjection {
    /// <summary>
    /// Contains extension methods to <see cref="IIdentityServerBuilder"/> for configuring identity server.
    /// </summary>
    public static class IIdentityServerBuilderExtensions {
        /// <summary>
        /// Adds the Asp.Net core identity.
        /// </summary>
        /// <typeparam name="TUser">The type of the t user.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <returns>IIdentityServerBuilder.</returns>
        public static IIdentityServerBuilder AddAspNetCoreIdentity<TUser>(this IIdentityServerBuilder builder)
            where TUser : class {
            return builder.AddAspNetCoreIdentity<TUser>("idsvr");
        }

        /// <summary>
        /// Adds the Asp.Net core identity.
        /// </summary>
        /// <typeparam name="TUser">The type of the t user.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <param name="authenticationScheme">The authentication scheme.</param>
        /// <returns>IIdentityServerBuilder.</returns>
        public static IIdentityServerBuilder AddAspNetCoreIdentity<TUser>(this IIdentityServerBuilder builder, string authenticationScheme)
            where TUser : class {
            builder.Services.Configure<IdentityServerOptions>(options => {
                options.AuthenticationOptions.AuthenticationScheme = authenticationScheme;
            });

            builder.Services.Configure<IdentityOptions>(options => {
                options.Cookies.ApplicationCookie.AuthenticationScheme = authenticationScheme;
                options.ClaimsIdentity.UserIdClaimType = JwtClaimTypes.Subject;
                options.ClaimsIdentity.UserNameClaimType = JwtClaimTypes.Name;
                options.ClaimsIdentity.RoleClaimType = JwtClaimTypes.Role;
            });

            builder.AddResourceOwnerValidator<IdentityResourceOwnerPasswordValidator<TUser>>();
            builder.AddProfileService<IdentityProfileService<TUser>>();

            builder.Services.AddTransient<ISecurityStampValidator, RigoFunc.IdentityServer.SecurityStampValidator<TUser>>();

            return builder;
        }
    }
}

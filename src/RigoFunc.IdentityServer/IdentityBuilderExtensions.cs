using Microsoft.AspNetCore.Identity;
using RigoFunc.IdentityServer;

namespace Microsoft.Extensions.DependencyInjection {
    public static class IdentityBuilderExtensions {
        public static IdentityBuilder AddUserClaimsPrincipalFactory(this IdentityBuilder builder) {
            var interfaceType = typeof(IUserClaimsPrincipalFactory<>).MakeGenericType(builder.UserType);

            var classType = typeof(UserClaimsFactory<,>).MakeGenericType(builder.UserType, builder.RoleType);

            builder.Services.AddScoped(interfaceType, classType);

            return builder;
        }
    }
}

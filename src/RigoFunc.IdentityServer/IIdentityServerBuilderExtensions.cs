﻿// Copyright (c) RigoFunc (xuyingting). All rights reserved.

using System;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Configuration;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RigoFunc.IdentityServer.Services;

namespace RigoFunc.IdentityServer {
    public static class IIdentityServerBuilderExtensions {
        public static IIdentityServerBuilder UseAspNetCoreIdentity<TUser>(this IIdentityServerBuilder builder) where TUser : class {
            var services = builder.Services;

            services.TryAddTransient<IEmailSender, MessageSender>();
            services.TryAddTransient<ISmsSender, MessageSender>();
            services.AddTransient<SignInManager<TUser>, IdentityServerSignInManager<TUser>>();
            services.AddTransient<IProfileService, IdentityProfileService>();
            services.AddTransient<IResourceOwnerPasswordValidator, IdentityResourceOwnerPasswordValidator<TUser>>();
            services.AddTransient<ICorsPolicyService, IdentityCorsPolicyService>();

            services.Configure<IdentityOptions>(options => {
                options.Cookies.ApplicationCookie.AuthenticationScheme = Constants.PrimaryAuthenticationType;

                options.Cookies.ApplicationCookie.LoginPath = new PathString("/" + Constants.RoutePaths.Login);
                options.Cookies.ApplicationCookie.LogoutPath = new PathString("/" + Constants.RoutePaths.Logout);

                options.ClaimsIdentity.UserIdClaimType = JwtClaimTypes.Subject;
                options.ClaimsIdentity.UserNameClaimType = JwtClaimTypes.Name;
                options.ClaimsIdentity.RoleClaimType = JwtClaimTypes.Role;
            });

            services.Configure<IdentityServerOptions>(
               options => {
                   options.AuthenticationOptions.PrimaryAuthenticationScheme = Constants.PrimaryAuthenticationType;
               });

            return builder;
        }
    }
}

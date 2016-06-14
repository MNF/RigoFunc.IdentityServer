﻿using System;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RigoFunc.OAuth;
using RigoFunc.OAuth.Services;

namespace Microsoft.Extensions.DependencyInjection {
    /// <summary>
    /// Contains extension methods to <see cref="IServiceCollection"/> for configuring OAuth services.
    /// </summary>
    public static class OAuthServiceCollectionExtensions {
        /// <summary>
        /// Adds OAuth server to <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The services available in the application.</param>
        /// <param name="setupAction">An action to configure the <see cref="OAuthServerOptions"/>.</param>
        /// <returns>The services available in the application.</returns>
        public static IServiceCollection AddOAuth(this IServiceCollection services, Action<OAuthServerOptions> setupAction) {
            services.AddAuthorization();
            services.AddTransient<IEmailSender, MessageSender>();
            services.AddTransient<ISmsSender, MessageSender>();

            if (setupAction != null) {
                services.Configure(setupAction);
            }

            return services;
        }
    }
}

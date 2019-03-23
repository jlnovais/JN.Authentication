using System;
using Microsoft.AspNetCore.Authentication;

namespace JN.Authentication.Scheme
{
    public static class ApiKeyAuthenticationExtensions
    {
        public static AuthenticationBuilder AddApiKey(this AuthenticationBuilder builder)
        {
            return AddApiKey(builder, ApiKeyAuthenticationDefaults.AuthenticationScheme, _ => { });
        }

        public static AuthenticationBuilder AddApiKey(this AuthenticationBuilder builder,
            string authenticationScheme)
        {
            return AddApiKey(builder, authenticationScheme, _ => { });
        }

        public static AuthenticationBuilder AddApiKey(this AuthenticationBuilder builder,
            Action<ApiKeyAuthenticationOptions> configureOptions)
        {
            return AddApiKey(builder, ApiKeyAuthenticationDefaults.AuthenticationScheme, configureOptions);
        }

        public static AuthenticationBuilder AddApiKey(this AuthenticationBuilder builder,
            string authenticationScheme, Action<ApiKeyAuthenticationOptions> configureOptions)
        {
            //builder.Services.AddTransient<IApiKeyValidationService, TAuthService>();

            return builder.AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(authenticationScheme, configureOptions);
        }
    }
}
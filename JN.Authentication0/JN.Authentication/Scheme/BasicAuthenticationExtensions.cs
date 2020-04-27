using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace JN.Authentication.Scheme
{
    public static class BasicAuthenticationExtensions
    {
        public static AuthenticationBuilder AddBasic(this AuthenticationBuilder builder)
        {
            return AddBasic(builder, BasicAuthenticationDefaults.AuthenticationScheme, _ => { });
        }

        public static AuthenticationBuilder AddBasic(this AuthenticationBuilder builder,
            string authenticationScheme)
        {
            return AddBasic(builder, authenticationScheme, _ => { });
        }

        public static AuthenticationBuilder AddBasic(this AuthenticationBuilder builder,
            Action<BasicAuthenticationOptions> configureOptions, string tag = null)
        {
            return AddBasic(builder, BasicAuthenticationDefaults.AuthenticationScheme, configureOptions, tag);
        }

        public static AuthenticationBuilder AddBasic(this AuthenticationBuilder builder,
            string authenticationScheme, Action<BasicAuthenticationOptions> configureOptions, string tag = null)
        {
            builder.Services
                .AddSingleton<IPostConfigureOptions<BasicAuthenticationOptions>, BasicAuthenticationPostConfigureOptions>();

            if (!string.IsNullOrWhiteSpace(tag))
                authenticationScheme += tag;

            return builder.AddScheme<BasicAuthenticationOptions, BasicAuthenticationHandler>(authenticationScheme, configureOptions);
        }

    }
}
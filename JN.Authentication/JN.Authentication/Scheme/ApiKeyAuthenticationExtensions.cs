﻿using System;
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
            Action<ApiKeyAuthenticationOptions> configureOptions, string tag = null)
        {
            return AddApiKey(builder, ApiKeyAuthenticationDefaults.AuthenticationScheme, configureOptions, tag);
        }

        public static AuthenticationBuilder AddApiKey(this AuthenticationBuilder builder,
            string authenticationScheme, Action<ApiKeyAuthenticationOptions> configureOptions, string tag = null)
        {
            if (!string.IsNullOrWhiteSpace(tag))
                authenticationScheme += tag;

            return builder.AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(authenticationScheme, configureOptions);
        }
    }
}
using System;
using Microsoft.Extensions.Options;

namespace JN.Authentication.Scheme
{
    public class BasicAuthenticationPostConfigureOptions : IPostConfigureOptions<BasicAuthenticationOptions>
    {
        public void PostConfigure(string name, BasicAuthenticationOptions options)
        {
            if (string.IsNullOrWhiteSpace(options.Realm))
            {
                throw new InvalidOperationException("Realm must be provided in options");
            }
        }
    }
}
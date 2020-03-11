using System;
using System.Threading.Tasks;
using JN.Authentication.HelperClasses;
using Microsoft.AspNetCore.Authentication;

namespace JN.Authentication.Scheme
{
    public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
    {
        public bool LogInformation { get; set; }
        public bool HttpPostMethodOnly { get; set; }
        public string HeaderName { get; set; } = "ApiKey";

        public bool AcceptsQueryString { get; set; } = false;

        /// <summary>
        /// Called to validate a api key.
        /// </summary>
        [Obsolete("This property is obsolete. Use IApiKeyValidationService instead.", false)]
        public Func<string, Task<ValidationResult>> ValidateKey { get; set; }
        /// <summary>
        /// Called before a 401 response is sent to the client
        /// </summary>
        public Func<Exception, Task<ChallengeResult>> ChallengeResponse { get; set; }
    }
}
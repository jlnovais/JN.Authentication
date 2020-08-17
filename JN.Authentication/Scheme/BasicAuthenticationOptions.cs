using System;
using System.Text;
using System.Threading.Tasks;
using JN.Authentication.HelperClasses;
using Microsoft.AspNetCore.Authentication;

namespace JN.Authentication.Scheme
{
    public class BasicAuthenticationOptions : AuthenticationSchemeOptions
    {
        public string Realm { get; set; }

        /// <summary>
        /// Log information in Warning and Error levels (Info level is used internally by AuthenticateResult). Configure log levels in log provider to write info in separate files.
        /// </summary>
        public bool LogInformation { get; set; }

        public Encoding HeaderEncoding { get; set; } = Encoding.UTF8;

        public bool HttpPostMethodOnly { get; set; }

        /// <summary>
        /// Called to validate a user. Usage: ValidateUser(username,password,realm)
        /// </summary>
        [Obsolete("This property is obsolete. Use IBasicValidationService instead.", false)]
        public Func<string, string, string, Task<ValidationResult>> ValidateUser { get; set; }

        /// <summary>
        /// Called before a 401 response is sent to the client
        /// </summary>
        public Func<Exception, Task<ChallengeResult>> ChallengeResponse { get; set; }

    }
}
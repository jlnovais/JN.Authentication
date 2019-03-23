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
        public bool LogInformation { get; set; }
        public Encoding HeaderEncoding { get; set; } = Encoding.UTF8;
        public bool HttpPostMethodOnly { get; set; }

        /// <summary>
        /// Called to validate a user. Usage: ValidateUser(username,password,realm)
        /// </summary>
        public Func<string, string, string, Task<ValidationResult>> ValidateUser { get; set; }

        /// <summary>
        /// Called before a 401 response is sent to the client
        /// </summary>
        public Func<Exception, Task<ChallengeResult>> ChallengeResponse { get; set; }
    }
}
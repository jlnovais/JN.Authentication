using System;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using JN.Authentication.HelperClasses;
using JN.Authentication.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JN.Authentication.Scheme
{
    public class BasicAuthenticationHandler : AuthenticationHandler<BasicAuthenticationOptions>
    {
        private readonly IBasicValidationService _validationService;
        private const string AuthorizationHeaderName = "Authorization";
        private const string SchemeName = "Basic";

        public BasicAuthenticationHandler(
            IOptionsMonitor<BasicAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IBasicValidationService validationService = null)
            : base(options, logger, encoder, clock)
        {
            _validationService = validationService;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {

            if (Request.Method != "POST" && Options.HttpPostMethodOnly)
            {
                if (Options.LogInformation) 
                    Logger.LogError("HTTP Method Not Allowed");

                return AuthenticateResult.Fail(new CustomAuthException("HTTP Method Not Allowed", AuthenticationError.MethodNotAllowed));
            }


            if (!AuthenticationHeaderValue.TryParse(Request.Headers[AuthorizationHeaderName],
                out AuthenticationHeaderValue headerValue))
            {
                return AuthenticateResult.NoResult();
            }

            if (!SchemeName.Equals(headerValue.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                if (Options.LogInformation) 
                    Logger.LogError("Not Basic authentication header");

                return AuthenticateResult.NoResult();
            }

            string[] userDetails;
            try
            {
                var headerValueBytes = Convert.FromBase64String(headerValue.Parameter);
                var userAndPassword = Options.HeaderEncoding.GetString(headerValueBytes);
                userDetails = userAndPassword.Split(':');
            }
            catch (Exception exception)
            {
                var msg = $"Invalid Basic authentication header - {exception.Message}";

                if (Options.LogInformation) 
                    Logger.LogError(msg);

                return AuthenticateResult.Fail(msg);
            }

            if (userDetails.Length != 2)
            {
                var msg = "Invalid Basic authentication header - missing username or password. ";
                
                if (Options.LogInformation)
                    Logger.LogError(msg);

                return AuthenticateResult.Fail(msg);
            }

            string user = userDetails[0];
            string password = userDetails[1];

            ValidationResult userValidationResult;
            try
            {
                if (_validationService != null)
                    userValidationResult = await _validationService.ValidateUser(user, password, Options.Realm);
                else
                    userValidationResult = await Options.ValidateUser(user, password, Options.Realm);


                if (!userValidationResult.Success && userValidationResult.ErrorCode != 0)
                    return AuthenticateResult.Fail(new CustomAuthException(userValidationResult.ErrorDescription,
                        userValidationResult.ErrorCode));
            }

            catch (Exception ex)
            {
                var msg = $"Error validating user. Details: {ex.Message}";

                if (Options.LogInformation)
                    Logger.LogError(msg);

                return AuthenticateResult.Fail(new CustomAuthException(msg, ex, AuthenticationError.OtherError));
            }


            if (!userValidationResult.Success)
            {
                var msg = $"Invalid username or password. Username: {user}";

                if (Options.LogInformation)
                    Logger.LogError(msg);

                return AuthenticateResult.Fail(new CustomAuthException(msg, AuthenticationError.AuthenticationFailed));
            }

            if (Options.LogInformation)
                Logger.LogWarning($"User authenticated: {user}");

            var claims = userValidationResult.Claims;

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }


        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            const int defaultStatus = (int)HttpStatusCode.Unauthorized;

            Response.StatusCode = defaultStatus;

            if (Options.ChallengeResponse == null)
                return;

            var authResult = await this.HandleAuthenticateOnceSafeAsync();

            if (authResult.Succeeded)
            {
                return;
            }

            var result = await Options.ChallengeResponse(authResult.Failure);

            Response.StatusCode = result.StatusCode >= 200 ? result.StatusCode : defaultStatus;

            if (Response.StatusCode == defaultStatus)
                Response.Headers["WWW-Authenticate"] = $"Basic realm=\"{Options.Realm}\", charset=\"{Options.HeaderEncoding.HeaderName}\"";

            if (!string.IsNullOrWhiteSpace(result.TextToWriteOutput))
                await Response.WriteAsync(result.TextToWriteOutput);

        }
    }
}

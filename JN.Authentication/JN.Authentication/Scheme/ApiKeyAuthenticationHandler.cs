using System;
using System.Net;
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
    public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
    {
        private readonly IApiKeyValidationService _validationService;

        public ApiKeyAuthenticationHandler(
            IOptionsMonitor<ApiKeyAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IApiKeyValidationService validationService = null)
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

            var key = Request.Headers[Options.HeaderName];

            if (string.IsNullOrWhiteSpace(key) && Options.AcceptsQueryString)
                Request.Query.TryGetValue(Options.HeaderName, out key);

            if (string.IsNullOrWhiteSpace(key))
            {
                if (Options.LogInformation)
                    Logger.LogError("Invalid Api Key");
                return AuthenticateResult.NoResult();
            }

            ValidationResult userValidationResult;

            try
            {
                if (_validationService != null)
                    userValidationResult = await _validationService.ValidateApiKey(key);
                else
                    userValidationResult = await Options.ValidateKey(key);

                if (!userValidationResult.Success && userValidationResult.ErrorCode != 0)
                    return AuthenticateResult.Fail(new CustomAuthException(userValidationResult.ErrorDescription,
                        userValidationResult.ErrorCode));
            }
            catch (Exception ex)
            {
                var msg = $"Error validating API Key. Details: {ex.Message}";

                if (Options.LogInformation)
                    Logger.LogError(msg);

                return AuthenticateResult.Fail(new CustomAuthException(msg, ex, AuthenticationError.OtherError));
            }


            if (!userValidationResult.Success)
            {
                var msg = $"API Key not authenticated. Key {key}";

                if (Options.LogInformation)
                    Logger.LogError(msg);

                return AuthenticateResult.Fail(new CustomAuthException(msg, AuthenticationError.AuthenticationFailed));
            }

            if (Options.LogInformation)
                Logger.LogInformation($"API Key authenticated: {key}");

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

            Response.StatusCode = result.statusCode >= 200 ? result.statusCode : defaultStatus;

            if (!string.IsNullOrWhiteSpace(result.textToWriteOutput))
                await Response.WriteAsync(result.textToWriteOutput);
        }


    }
}

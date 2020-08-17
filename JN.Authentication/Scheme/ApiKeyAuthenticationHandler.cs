using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using JN.Authentication.HelperClasses;
using JN.Authentication.Interfaces;

namespace JN.Authentication.Scheme
{
    public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
    {
        private readonly IApiKeyValidationService _apiKeyValidationService;

        public ApiKeyAuthenticationHandler(
            IOptionsMonitor<ApiKeyAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IApiKeyValidationService apiKeyValidationService = null)
            : base(options, logger, encoder, clock)
        {
            _apiKeyValidationService = apiKeyValidationService;
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
                if (_apiKeyValidationService != null)
                    userValidationResult = await _apiKeyValidationService.ValidateApiKey(key);
                else
                    userValidationResult = await Options.ValidateKey(key);

                if (!userValidationResult.Success && userValidationResult.ErrorCode != 0)
                {
                    return AuthenticateResult.Fail(new CustomAuthException(userValidationResult.ErrorDescription,
                        userValidationResult.ErrorCode));
                }

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

            var ticket = GetAuthenticationTicket(claims);

            return AuthenticateResult.Success(ticket);
        }

        private AuthenticationTicket GetAuthenticationTicket(IEnumerable<Claim> claims)
        {
            var identity = new ClaimsIdentity(claims, this.Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, this.Scheme.Name);
            return ticket;
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

            var result = await Options.ChallengeResponse(authResult.Failure, new RequestDetails()
            {
                Path = Request.Path,
                ContentType = Request.ContentType,
                Host = Request.Host,
                Method = Request.Method,
                QueryString = Request.QueryString,
                Scheme = Request.Scheme
            });

            Response.StatusCode = GetResponseStatusCode(result, defaultStatus);

            if (!string.IsNullOrWhiteSpace(result.ContentType))
                Response.ContentType = result.ContentType;

            if (!string.IsNullOrWhiteSpace(result.TextToWriteOutput))
                await Response.WriteAsync(result.TextToWriteOutput);
        }

        private static int GetResponseStatusCode(ChallengeResult result, int defaultStatus)
        {
            return result.StatusCode >= 200 ? result.StatusCode : defaultStatus;
        }
    }
}

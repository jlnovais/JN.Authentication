using System;
using System.Security.Claims;
using System.Threading.Tasks;
using JN.Authentication.HelperClasses;
using JN.Authentication.Interfaces;

namespace JN.Authentication.APITest.Services
{
    public class ApiKeyValidationService : IApiKeyValidationService
    {
        public Task<ValidationResult> ValidateApiKey(string apiKey)
        {
            if (apiKey == "exception")
                throw new ArgumentException("specified key causes an exception");

            var claims = new[]
            {
                new Claim(ClaimTypes.GivenName, "Jose Teste"),
                new Claim(ClaimTypes.Name, "user1"),
                new Claim(ClaimTypes.Email, "email@email.com"),
                new Claim("IsAdmin", true.ToString()),
                new Claim(ClaimTypes.Role, "role1;role2")
            };

            var res = new ValidationResult
            {
                Success = (apiKey == "123"),
                ErrorDescription = (apiKey == "1234") ? "An error has occured" : "",
                ErrorCode = (apiKey == "1234") ? -1 : 0,
                Claims = claims
            };

            return Task.FromResult(res);
        }


    }
}

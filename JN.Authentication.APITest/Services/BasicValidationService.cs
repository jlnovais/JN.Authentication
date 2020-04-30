using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using JN.Authentication.HelperClasses;
using JN.Authentication.Interfaces;

namespace JN.Authentication.APITest.Services
{
    public class BasicValidationService : IBasicValidationService
    {
        private readonly string _validUser = "test";
        private readonly string _validPassword = "123";

        private readonly string _errorUser = "testError";

        private readonly string _exceptionUsername = "exception";

        public Task<ValidationResult> ValidateUser(string username, string password, string resourceName)
        {
            if (username == _exceptionUsername)
            {
                throw new ArgumentException("specified user causes an exception");
            }

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
                Success = (username == _validUser && password == _validPassword),
                ErrorDescription = (username == _errorUser) ? "An error has occured" : "",
                ErrorCode = (username == _errorUser) ? -1 : 0,
                Claims = claims
            };

            return Task.FromResult(res);
        }


    }
}

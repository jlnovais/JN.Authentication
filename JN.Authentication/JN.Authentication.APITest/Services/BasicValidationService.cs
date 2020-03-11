using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using JN.Authentication.HelperClasses;
using JN.Authentication.Interfaces;

namespace JN.Authentication.APITest.Services
{
    public class BasicValidationService : IBasicValidationService
    {
        public Task<ValidationResult> ValidateUser(string username, string password, string resourceName)
        {
            if (username == "exception")
                throw new Exception("specified user causes an exception");

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
                Success = (username == "test" && password == "123"),
                ErrorDescription = (username == "testError") ? "An error has occured" : "",
                ErrorCode = (username == "testError") ? -1 : 0,
                Claims = claims
            };

            return Task.FromResult(res);
        }


    }
}

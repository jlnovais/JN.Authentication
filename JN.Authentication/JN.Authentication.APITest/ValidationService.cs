using System;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using JN.Authentication.HelperClasses;

namespace JN.Authentication.APITest
{

    public class ValidationService
    {

        public static Task<ChallengeResult> ChallengeResponse(Exception ex)
        {
            var res = new ChallengeResult();

            if (ex == null)
                return  Task.FromResult(res);

            if (ex is CustomAuthException exception)
            {
                var errorCode = (AuthenticationError) exception.ErrorCode;

                switch (errorCode)
                {
                    case AuthenticationError.MethodNotAllowed:
                        res.statusCode = (int)HttpStatusCode.MethodNotAllowed;
                        res.textToWriteOutput = "error was: Method Not Allowed";
                        break;
                    case AuthenticationError.OtherError:
                        res.statusCode = (int)HttpStatusCode.BadRequest;
                        res.textToWriteOutput = exception.Message;
                        break;

                    case (AuthenticationError)(-1):
                        res.statusCode = (int)HttpStatusCode.RequestTimeout;
                        res.textToWriteOutput = exception.Message;
                        break;
                    default:
                        res.textToWriteOutput = ex.Message;
                        break;
                }
            }
            else
            {
                res.textToWriteOutput = ex.Message;
            }

            return  Task.FromResult(res);
        }

        public static Task<ValidationResult> ValidateApiKey(string key)
        {

            if (key == "exception")
                throw new Exception("specified key causes an exception");

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
                Success = (key == "123"),
                ErrorDescription = (key == "1234") ? "An error has occured": "",
                ErrorCode = (key == "1234") ? -1:0,
                Claims = claims
            };

            return Task.FromResult(res);
        }

        //------------------------------------------


        public static Task<ValidationResult> ValidateUser(string username, string password, string realm)
        {

            if(username == "exception")
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
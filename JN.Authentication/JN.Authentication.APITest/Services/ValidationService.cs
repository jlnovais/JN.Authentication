using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using JN.Authentication.HelperClasses;

namespace JN.Authentication.APITest.Services
{
    public class ValidationService
    {

        public static Task<ChallengeResult> ChallengeResponse(Exception ex)
        {
            var res = new ChallengeResult();

            if (ex == null)
                return Task.FromResult(res);

            if (ex is CustomAuthException exception)
            {
                var errorCode = (AuthenticationError)exception.ErrorCode;

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

            return Task.FromResult(res);
        }


    }
}

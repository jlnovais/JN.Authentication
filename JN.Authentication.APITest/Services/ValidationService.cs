using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using JN.Authentication.HelperClasses;

namespace JN.Authentication.APITest.Services
{
    public static class ValidationService
    {

        public static Task<ChallengeResult> ChallengeResponse(Exception ex, RequestDetails requestDetails)
        {
            var res = new ChallengeResult();

            if (ex == null)
            {
                return Task.FromResult(res);
            }

            if (ex is CustomAuthException exception)
            {
                var errorCode = (AuthenticationError)exception.ErrorCode;

                switch (errorCode)
                {
                    case AuthenticationError.MethodNotAllowed:
                        res.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                        res.TextToWriteOutput = "error was: Method Not Allowed";
                        res.ContentType = "application/test";
                        break;
                    case AuthenticationError.OtherError:
                        res.StatusCode = (int)HttpStatusCode.BadRequest;
                        res.TextToWriteOutput = exception.Message;
                        res.ContentType = "application/test";
                        break;
                    case AuthenticationError.AuthenticationFailed:
                        res.StatusCode = (int) HttpStatusCode.Unauthorized;
                        res.TextToWriteOutput = exception.Message;
                        res.ContentType = "application/test";
                        break;
                    case (AuthenticationError)(-1):
                        res.StatusCode = (int)HttpStatusCode.RequestTimeout;
                        res.TextToWriteOutput = exception.Message;
                        res.ContentType = "application/test";
                        break;
                    default:
                        res.TextToWriteOutput = ex.Message;
                        break;
                }
            }
            else
            {
                res.TextToWriteOutput = ex.Message;
            }

            return Task.FromResult(res);
        }


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace JN.Authentication.HelperClasses
{

    public class ChallengeResult
    {
        public int statusCode;
        public string textToWriteOutput;
    }

    public class ValidationResult
    {
        public bool Success;
        public int ErrorCode;
        public string ErrorDescription;
        public string ErrorDescription2;
        public IEnumerable<Claim> Claims;
    }
}

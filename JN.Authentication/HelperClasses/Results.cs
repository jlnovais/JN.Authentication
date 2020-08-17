using System.Collections.Generic;
using System.Security.Claims;


namespace JN.Authentication.HelperClasses
{

    public class ChallengeResult
    {
        public int StatusCode { get; set; }
        public string TextToWriteOutput { get; set; }

        public string ContentType { get; set; }
    }

    public class ValidationResult
    {
        public bool Success { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string ErrorDescription2 { get; }
        public IEnumerable<Claim> Claims { get; set; }
    }
}

using System;
using System.Runtime.Serialization;

namespace JN.Authentication.HelperClasses
{
    public class CustomAuthException: Exception
    {
        public CustomAuthException()
        {
        }

        public CustomAuthException(string message) : base(message)
        {
        }

        public CustomAuthException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CustomAuthException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public CustomAuthException(string message, int errorCode) : base(message)
        {
            ErrorCode = errorCode;
        }

        public CustomAuthException(string message, AuthenticationError errorCode) : base(message)
        {
            ErrorCode = (int)errorCode;
        }
        

        public CustomAuthException(string message, Exception innerException, AuthenticationError errorCode) : base(message, innerException)
        {
            ErrorCode = (int)errorCode;
        }

        public int ErrorCode { get; set; }
            
        
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace JN.Authentication.HelperClasses
{
    public class RequestDetails
    {
        public PathString Path { get; set; }
        public string ContentType { get; set; }
        public HostString Host { get; set; }
        public string Method { get; set; }
        public QueryString QueryString { get; set; }
        public string Scheme { get; set; }
    }
}

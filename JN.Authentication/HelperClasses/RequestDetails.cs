using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

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
        public StringValues AcceptHeader { get; set; }
    }
}

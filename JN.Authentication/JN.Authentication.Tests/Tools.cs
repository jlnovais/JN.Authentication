using System;
using System.Net.Http;
using System.Text;

namespace JN.Authentication.Tests
{
    public class Tools
    {
        public static StringContent GetContent()
        {
            return new StringContent("{\"Name\": \"my name\",\"Address\": \"my address\"}",
                Encoding.UTF8,
                "application/json");
        }

        public static string BasicAuthCredentials(string username, string password)
        {

            string svcCredentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(username + ":" + password));

            return svcCredentials;

        }
    }
}

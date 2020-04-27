using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JN.Authentication.APITest.Services
{
    public interface ITestService
    {
        IEnumerable<string> GetValues();
        string GetValue(int id);
    }

    public class TestService : ITestService
    {
        public IEnumerable<string> GetValues()
        {
            return new[] { "value1", "value2" };
        }

        public string GetValue(int id)
        {
            return "value: " + id;
        }
    }
}

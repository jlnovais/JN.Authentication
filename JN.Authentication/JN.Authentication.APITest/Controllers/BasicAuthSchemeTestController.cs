using System.Collections.Generic;
using AuthenticationDemo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JN.Authentication.APITest.Controllers
{
    [Route("api/[controller]")]

    [Authorize(AuthenticationSchemes = "Basic", Policy = "IsAdminPolicy")]
    [ApiController]

    public class BasicAuthSchemeTestController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public ActionResult<string> Post([FromBody] MyInputObject obj)
        {
            return "Post test OK - input string was: " + obj.ToString();
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
            //can be empty
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            //can be empty
        }
    }
}

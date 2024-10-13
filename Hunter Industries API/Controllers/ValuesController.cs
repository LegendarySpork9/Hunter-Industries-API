using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace HunterIndustriesAPI.Controllers
{
    /// <summary>
    /// </summary>
    public class ValuesController : ApiController
    {
        /// <summary>
        /// </summary>
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        /// <summary>
        /// </summary>
        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        /// <summary>
        /// </summary>
        // POST api/values
        public void Post([FromBody] string value)
        {
        }

        /// <summary>
        /// </summary>
        // PUT api/values/5
        public void Put(int id, [FromBody] string value)
        {
        }

        /// <summary>
        /// </summary>
        // DELETE api/values/5
        public void Delete(int id)
        {
        }

        /// <summary>
        /// </summary>
        public void Patch(int id, [FromBody] string value)
        {
        }
    }
}

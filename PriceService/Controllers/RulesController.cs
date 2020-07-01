using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PriceService.Controllers
{
    public class RulesController : ApiController
    {
        // GET: api/Rules
        public IEnumerable<Models.PriceRule> Get()
        {
            
            return Models.PriceRules.Rules;
        }

        // GET: api/Rules/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Rules
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Rules/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Rules/5
        public void Delete(int id)
        {
        }
    }
}

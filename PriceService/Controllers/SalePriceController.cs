using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PriceService.Controllers
{
    public class SalePriceController : ApiController
    {
        // GET: api/SalePrice
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/SalePrice/5
        public string Get(int batchId)
        {
            return "value";
        }

        // POST: api/SalePrice
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/SalePrice/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/SalePrice/5
        public void Delete(int id)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using quoteService.DTO;
using quoteService.Models;

namespace quoteService.Controllers
{
    [EnableCors("*", "*", "*")]
    [RoutePrefix("api/customer")]
    public class CustomerController : ApiController
    {
        private HillsStock1Entities db = new HillsStock1Entities();

        // GET: api/Customer
        [Route("all")]
        public IEnumerable<CustomerDTO> GetAllQuotes()
        {

            var dto = db.CustomerInformations.Select(item => new DTO.CustomerDTO
            {
                CustomerId = item.CustomerId,
                CustomerReference = item.CustomerReference,
                CustomerName = item.CustomerName,
                CustomerTel = item.CustomerTel,
                CustomerAddress = item.CustomerAddress,
                CustomerEmail = item.CustomerEmail,
                SageCustomer = item.SageCustomer,
            }).AsEnumerable();
            return dto;
        }

        [Route("single")]
        public CustomerDTO GetOneCustomer(string customerReference)
        {
            var justOne = db.CustomerInformations.SingleOrDefault(q => q.CustomerReference == customerReference);

            var dto = new DTO.CustomerDTO
            {
                CustomerId = justOne.CustomerId,
                CustomerReference = justOne.CustomerReference,
                CustomerName = justOne.CustomerName,
                CustomerTel = justOne.CustomerTel,
                CustomerAddress = justOne.CustomerAddress,
                CustomerEmail = justOne.CustomerEmail,
                SageCustomer = justOne.SageCustomer,
            };
            return dto;
        }

        // GET: api/Customer/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Customer
        [ResponseType(typeof(CustomerDTO))]
        public HttpResponseMessage PostCustomer(HttpRequestMessage request, CustomerDTO customer)
        {
            if (!ModelState.IsValid)
            {
                return request.CreateResponse(HttpStatusCode.BadRequest, customer);
            }
            try
            {
                var newCust = new CustomerInformation();
                newCust.CustomerReference = customer.CustomerReference;
                newCust.CustomerName = customer.CustomerName;
                newCust.CustomerTel = customer.CustomerTel;
                newCust.CustomerAddress = customer.CustomerAddress;
                newCust.CustomerEmail = customer.CustomerEmail;

                db.CustomerInformations.Add(newCust);
                db.SaveChanges();
      
                return request.CreateResponse(HttpStatusCode.OK, customer);
            }
            catch (Exception ex)
            {
                return request.CreateResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        // PUT: api/Customer/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Customer/5
        public void Delete(int id)
        {
        }
    }
}

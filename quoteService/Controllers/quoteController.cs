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
    [RoutePrefix("api/quote")]
    public class quoteController : ApiController
    {

        private HillsStock1Entities db = new HillsStock1Entities();

        // Will be used to display all of the quotes that are currently in the system.
        // GET: api/quote/all
        [Route("all")]
        public IEnumerable<QuoteDTO> GetAllQuotes()
        {
            
            var dto = db.Quotes.Select(item => new DTO.QuoteDTO
            {
                QuoteId = item.QuoteId,
                CustomerRef = item.CustomerReference,
                Date = item.QuoteDate.ToString(),
                ExpiryDate = item.QuoteExpiryDate.ToString(),
                SiteRef = item.SiteReference,
                TotalPrice = item.QuotePrice ?? 0,
                Active = item.Active,
        }).AsEnumerable();
            return dto;
        }

        [Route("customer")]
        public IEnumerable<QuoteDTO> GetCustomerQuotes(string CustomerReference)
        {

            var dto = db.Quotes.Where(c => c.CustomerReference.ToLower() == CustomerReference.ToLower()).Select(item => new DTO.QuoteDTO
            {
                QuoteId = item.QuoteId,
                CustomerRef = item.CustomerReference,
                Date = item.QuoteDate.ToString(),
                ExpiryDate = item.QuoteExpiryDate.ToString(),
                SiteRef = item.SiteReference,
                TotalPrice = item.QuotePrice ?? 0,
                Active = item.Active,
            }).AsEnumerable();
            return dto;
        }

        // GET: api/quote/detail?id={id}
        [Route("detail")]
        public SelectedQuoteDTO GetCustomerQuoteById(int id)
        {
            var justOne = db.Quotes.SingleOrDefault(q => q.QuoteId == id);

            var plantsForQuote = justOne.PlantsForQuotes.Select(item => new QuoteDetailDTO
            {
                Quantity = item.Quantity ?? 0,
                PlantName = item.PlantName,
                FormSize = item.FormSize,
                Comment = item.Comment,
                Price = item.Price ?? 0,
            });


            var dto = new DTO.SelectedQuoteDTO
            {
                QuoteId = justOne.QuoteId,
                CustomerRef = justOne.CustomerReference,
                TotalPrice = justOne.QuotePrice ?? 0,
                QuoteDetails = plantsForQuote
            };

            return dto;
        }

        // GET: api/quote/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/quote
        [ResponseType(typeof(NewQuoteDTO))]
        public HttpResponseMessage Post(HttpRequestMessage request, [FromBody]NewQuoteDTO quote)
        {
            if (!ModelState.IsValid)
            {
               return request.CreateResponse(HttpStatusCode.BadRequest, quote);
            }

            try
            {
                var thisQuote = new Quote();
                thisQuote.Active = quote.Active;
                thisQuote.CustomerReference = quote.CustomerRef;
                thisQuote.QuoteDate = DateTime.Parse(quote.Date);
                thisQuote.QuoteExpiryDate = DateTime.Parse(quote.ExpiryDate);
                thisQuote.SiteReference = quote.SiteRef;
                thisQuote.QuotePrice = quote.TotalPrice;

                db.Quotes.Add(thisQuote);
                db.SaveChanges();
                db.Entry(thisQuote).Reload();

                var quoteId = thisQuote.QuoteId;
                foreach (var plant in quote.QuoteDetails)
                {

                    var thisPlant = new PlantsForQuote();
                    thisPlant.Comment = plant.Comment;
                    thisPlant.FormSize = plant.FormSize;
                    thisPlant.PlantName = plant.PlantName;
                    thisPlant.Price = plant.Price;
                    thisPlant.Quantity = plant.Quantity;
                    thisPlant.QuoteId = quoteId;
                    db.PlantsForQuotes.Add(thisPlant);
                }
                db.SaveChanges();
                return request.CreateResponse(HttpStatusCode.OK, quote);
            }
            catch(Exception ex)
            {
                return request.CreateResponse(HttpStatusCode.BadRequest, quote);
            }
        }

        // PUT: api/quote/5
        public void Put(int id, SelectedQuoteDTO quote)
        {
        }

        // DELETE: api/quote/5
        public void Delete(int id)
        {
        }
    }
}

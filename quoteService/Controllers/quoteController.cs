using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Globalization;
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
            DateTime d = DateTime.Now;

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
                PlantForQuoteId = item.PlantsForQuoteId,
                Quantity = item.Quantity ?? 0,
                PlantName = item.PlantName,
                FormSize = item.FormSize,
                Comment = item.Comment,
                Price = item.Price ?? 0,
                Active = item.Active,
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

        //// GET: api/quote/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

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
                //ParseExact is used as when trying to pass in dates from client side in set format, it needed changed to be saved into db?
                thisQuote.QuoteDate = DateTime.ParseExact(quote.Date, "dd/MM/yyyy", CultureInfo.InvariantCulture); 
                thisQuote.QuoteExpiryDate = DateTime.ParseExact(quote.ExpiryDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
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
                    thisPlant.Active = plant.Active;
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

        // PUT: api/quote/edit?={id}
        [Route("edit")]
        [ResponseType(typeof(void))]
        public HttpResponseMessage PutEditQuote(HttpRequestMessage request, int id, SelectedQuoteDTO quote)
        {

            if (!ModelState.IsValid)
            {
                return request.CreateResponse(HttpStatusCode.BadRequest, quote);
            }

            // get the object out of the db
            var q = db.Quotes.Find(id);

            if (id != quote.QuoteId)
            {
                return request.CreateResponse(HttpStatusCode.BadRequest, quote);
            }

            if (quote.CustomerRef != null)
            {
                q.CustomerReference = quote.CustomerRef;
            }
            if (quote.TotalPrice != 0)
            {
                q.QuotePrice = quote.TotalPrice;
            }
            if (quote.SiteRef != null)
            {
                q.SiteReference = quote.SiteRef;
            }
            
            foreach (var plant in quote.QuoteDetails) //Loop through the array of plants that come with the quote
            {
                //Get a reference of the current plant that needs to be edited
                var plantToChange = db.PlantsForQuotes.SingleOrDefault(p =>p.PlantsForQuoteId == plant.PlantForQuoteId);
                if (plantToChange == null)
                {
                    return request.CreateResponse(HttpStatusCode.NotFound, quote);
                }
                if(plant.Comment != null)
                {
                    plantToChange.Comment = plant.Comment;
                }
                if(plant.Price != 0)
                {
                    plantToChange.Price = plant.Price;
                }
                if(plant.Quantity != 0)
                {
                    plantToChange.Quantity = plant.Quantity;
                }
                //Change the active flag because unless deleted on client side, it will be true
                plantToChange.Active = plant.Active;
                
            }

            db.Entry(q).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuoteExists(id))
                {
                    return request.CreateResponse(HttpStatusCode.NotFound, quote);
                }
                else
                {
                    throw;
                }
            }

            return request.CreateResponse(HttpStatusCode.OK, quote);
        }


        // PUT: api/quote/delete?={id}
        [Route("delete")]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutQuote(int id, QuoteDTO quote)
        {

            if (!ModelState.IsValid) 
            { 
                return BadRequest(ModelState);
            }

            // get the object out of the db
            var q = db.Quotes.Find(id);

            if (id != quote.QuoteId)
            {
                return BadRequest();
            }

            //Set the current value of active to be false
            q.Active = quote.Active;
         
            db.Entry(q).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuoteExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // DELETE: api/quote/5
        public void Delete(int id)
        {
        }

        private bool QuoteExists(int id)
        {
            return db.Quotes.Count(e => e.QuoteId == id) > 0;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Drawing.Design;
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

        #region getMethods
        // Will be used to display all of the quotes that are currently in the system.
        // GET: api/quote/all
        [Route("all")]
        public IEnumerable<QuoteDTO> GetAllQuotes()
        {

            List<QuoteDTO> DTO = new List<QuoteDTO>();
            var customers = db.CustomerInformations.ToList();

            var quotes = db.Quotes.ToList();

            foreach(Quote item in quotes)
            {
                var thisC = getCustomer(customers, item);

                var totalPicklistQuantity = 0;

                var picklistExists = db.Picklists.Any(x => x.QuoteId == item.QuoteId);
                if (picklistExists)
                {
                    var picklists = db.Picklists.Where(x => x.QuoteId == item.QuoteId);
                    foreach(var picklist in picklists) 
                    {
                        var toAdd = db.PlantsForPicklists.Where(y => y.PicklistId == picklist.PicklistId).Sum(y => y.Active == true ? y.QuantityToPick : 0);
                        totalPicklistQuantity += toAdd;
                    }
                }

                var Thisdto = new DTO.QuoteDTO
                {
                    QuoteId = item.QuoteId,
                    CustomerRef = item.CustomerReference,
                    CustomerAddress = thisC.CustomerAddress,
                    CustomerName = thisC.CustomerName,
                    CustomerTel = thisC.CustomerTel == "" ? null : thisC.CustomerTel,
                    CustomerTel2 = thisC.CustomerTel2 == "" ? null : thisC.CustomerTel2,
                    Date = item.QuoteDate.ToString(),
                    ExpiryDate = item.QuoteExpiryDate.ToString(),
                    SiteRef = item.SiteReference,
                    TotalPrice = item.QuotePrice ?? 0,
                    SalesOrder = item.SalesOrder,
                    Retail = item.Retail,
                    Active = item.Active,
                    TotalQuoteQuantity = db.PlantsForQuotes.Where(x => x.QuoteId == item.QuoteId).Sum(x => x.Quantity) ?? 0,
                    TotalPicklistQuantity = totalPicklistQuantity,
                };
                DTO.Add(Thisdto);
            }

            //get customer reference related to quote
            //Create customerInfo DTO
            //Pass that DTO through this one to be accessed as an object
            //    IEnumerable<QuoteDTO> dto = db.Quotes.Select(item => new DTO.QuoteDTO
            //    {
            //        QuoteId = item.QuoteId,
            //        CustomerRef = getCustomer(customers,item),
            //        Date = item.QuoteDate.ToString(),
            //        ExpiryDate = item.QuoteExpiryDate.ToString(),
            //        SiteRef = item.SiteReference,
            //        TotalPrice = item.QuotePrice ?? 0,
            //        SalesOrder = item.SalesOrder,
            //        Retail = item.Retail,
            //        Active = item.Active,
            //}).AsEnumerable();

            return DTO;
        }

        private CustomerInformation getCustomer(IEnumerable<CustomerInformation> customers, Quote item)
        {

            var cs = customers.FirstOrDefault(c => c.CustomerReference == item.CustomerReference);
           if (cs != null)
           {
               return  cs;
            }
           else
            {
                return null;
            }
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

        #endregion getMethods

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
                thisQuote.SalesOrder = quote.SalesOrder;
                thisQuote.Retail = quote.Retail;

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

               
                return request.CreateResponse(HttpStatusCode.OK, quoteId);
            }
            catch(Exception ex)
            {
                return request.CreateResponse(HttpStatusCode.BadRequest, ex);
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
                #region TestId if id greater than 0 edit plant otherwise add plant to database
                if (plant.PlantForQuoteId > 0) //Check the incoming batches, if the PlantForQuoteId = -1, it means that it is a new plant on the quote
                {
                    //Get a reference of the current plant that needs to be edited
                    var plantToChange = db.PlantsForQuotes.SingleOrDefault(p => p.PlantsForQuoteId == plant.PlantForQuoteId);
                    if (plantToChange == null)
                    {
                        return request.CreateResponse(HttpStatusCode.NotFound, quote);
                    }
                    if (plant.Comment != null)
                    {
                        plantToChange.Comment = plant.Comment;
                    }
                    if (plant.Price != 0)
                    {
                        plantToChange.Price = plant.Price;
                    }
                    if (plant.Quantity != 0)
                    {
                        plantToChange.Quantity = plant.Quantity;
                    }
                    //Change the active flag because unless deleted on client side, it will be true
                    plantToChange.Active = plant.Active;

                    if (plantToChange.Active == false) //remove the plant from the picklist too
                    {
                        var plantForPicklistToEdit = db.PlantsForPicklists.FirstOrDefault(x => x.PlantForQuoteId == plantToChange.PlantsForQuoteId);

                        plantForPicklistToEdit.Active = false;
                    }

                } else
                {
                    addPlant(q, plant);
                }

                #endregion //end if 

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

        private void addPlant(Quote q, QuoteDetailDTO plant)
        {
            var thisPlant = new PlantsForQuote();
            thisPlant.Comment = plant.Comment;
            thisPlant.FormSize = plant.FormSize;
            thisPlant.PlantName = plant.PlantName;
            thisPlant.Price = plant.Price;
            thisPlant.Quantity = plant.Quantity;
            thisPlant.Active = plant.Active;
            thisPlant.QuoteId = q.QuoteId;
            db.PlantsForQuotes.Add(thisPlant);
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

            return StatusCode(HttpStatusCode.OK);
        }

        // PUT: api/quote/salesOrder?={id}
        [Route("salesOrder")]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutSOQuote(int id, QuoteDTO quote)
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
            q.SalesOrder = quote.SalesOrder;

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

            return StatusCode(HttpStatusCode.OK);
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

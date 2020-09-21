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
using QuoteService.DTO;

namespace QuoteService.Controllers
{
    [EnableCors("*", "*", "*")]
    [RoutePrefix("api/picklist")]
    public class PickListController : ApiController
    {
        private HillsStock1Entities db = new HillsStock1Entities();

        // GET: api/pickList/all
        [Route("all")]
        public IEnumerable<PickListDTO> GetAllPicklists()
        {
            var pickLists = db.Picklists.ToList();

            var picklistToRet = db.Picklists.Select(item => new DTO.PickListDTO
            {
                PicklistId = item.PicklistId,
                QuoteId = item.QuoteId,
                DispatchDate = item.DispatchDate.ToString(),
                DeliveryAddress = item.DeliveryAddress,
                DeliveryNeeded = item.DeliveryNeeded,
                CustomerName = db.CustomerInformations.FirstOrDefault(x => x.CustomerReference == db.Quotes.FirstOrDefault(z => z.QuoteId == item.QuoteId).CustomerReference).CustomerName ?? "No Customer Name",
                IsPicked = item.IsPicked,
                IsAllocated = item.IsAllocated,
                IsDelivered = item.IsDelivered,
                Comment = item.Comment,
                EstimatedDelivery = item.EstimatedDelivery, //Boolean because its either Estimated Or Exact Delivery Date
                PickListItemQty = db.PlantsForPicklists.Where(x => x.PicklistId == item.PicklistId).Sum(x => x.QuantityToPick),
                TotalAmountPicked = db.PlantsForPicklists.Where(x => x.PicklistId == item.PicklistId).Sum(x => x.QuantityPicked),
                Active = item.Active,
            }).AsEnumerable();

            return picklistToRet;
        }

        //public string GetCustomerName(int quoteId)
        //{
        //    var quoteRef = db.Quotes.SingleOrDefault(x => x.QuoteId == quoteId).CustomerReference;

        //    var customerName = db.CustomerInformations.SingleOrDefault(x => x.CustomerReference == quoteRef).CustomerName;
        //    if(customerName != null)
        //    {
        //        return customerName;
        //    } else
        //    {
        //        return " ";
        //    }
        //}

        // GET: api/picklist/detail?id={id}
        [Route("detail")]
        public SelectedPickListDTO GetPickListById(int id)
        {
            var justOne = db.Picklists.FirstOrDefault(q => q.PicklistId == id);

            var plantsForPicklist = justOne.PlantsForPicklists.Select(item => new PickListDetailDTO
            {
                PlantForPicklistId = item.PlantForPickListId,
                PicklistId = item.PlantForPickListId,
                PlantForQuoteId = item.PlantForQuoteId,
                BatchId = item.BatchId,
                BatchLocation = db.Batches.Any(x => x.Id == item.BatchId && x.Active == true) ? db.Batches.SingleOrDefault(x => x.Id == item.BatchId).Location : "No Location",
                PlantName = item.PlantName,
                FormSize = item.FormSize,
                QuantityToPick = item.QuantityToPick,
                QuantityPicked = item.QuantityPicked,
                IsSubbed = item.IsSubbed,
                DispatchLocation = item.DispatchLocation,
                Active = item.Active,
            });


            var dto = new DTO.SelectedPickListDTO
            {
                PicklistId = justOne.PicklistId,
                QuoteId = justOne.QuoteId,
                DispatchDate = justOne.DispatchDate.ToString(),
                DeliveryAddress = justOne.DeliveryAddress,
                DeliveryNeeded = justOne.DeliveryNeeded,
                CustomerRef = db.CustomerInformations.FirstOrDefault(x => x.CustomerReference == db.Quotes.FirstOrDefault(z => z.QuoteId == justOne.QuoteId).CustomerReference).CustomerReference ?? "No Customer Ref",
                CustomerTel = db.CustomerInformations.FirstOrDefault(x => x.CustomerReference == db.Quotes.FirstOrDefault(z => z.QuoteId == justOne.QuoteId).CustomerReference).CustomerTel ?? "No Customer Tel",
                IsPicked = justOne.IsPicked,
                IsAllocated = justOne.IsAllocated,
                IsDelivered = justOne.IsDelivered,
                Active = justOne.Active,
                Comment = justOne.Comment,
                EstimatedDelivery = justOne.EstimatedDelivery, //Boolean because its either Estimated Or Exact Delivery Date
                PickListItemQty = db.PlantsForPicklists.Where(x => x.PicklistId == justOne.PicklistId).Sum(x => x.QuantityToPick),
                TotalAmountPicked = db.PlantsForPicklists.Where(x => x.PicklistId == justOne.PicklistId).Sum(x => x.QuantityPicked),
                PickListPlants = plantsForPicklist,
            };

            return dto;
        }

        public class PlantsToReturn
        {
            public int PlantForQuoteId { get; set; }
            public string PlantName { get; set; }
            public string FormSize { get; set; }
            public int AmountNeeded { get; set; }
            public string Location { get; set; }
            public bool Active { get; set; }
        }

        // GET: api/picklist/plantsNeeded?id={id} <---Pass in the quoteId here?
        [Route("plantsNeeded")]
        public List<PlantsToReturn> GetRemainingQuotePlants(int id)
        {
            var plantsOnQuote = db.PlantsForQuotes.Where(x => x.QuoteId == id);
            List<PlantsToReturn> plantsReturn = new List<PlantsToReturn>();

            foreach (var plant in plantsOnQuote)
            {
                int plantOnPicklistQty;
                //var exists = db.PlantsForPicklists.Where(x => x.PlantForQuoteId == plant.PlantsForQuoteId).FirstOrDefault();
                var exists = db.PlantsForPicklists.Any(x => x.PlantForQuoteId == plant.PlantsForQuoteId);
                if (exists == true)
                {
                    plantOnPicklistQty = db.PlantsForPicklists.Where(x => x.PlantForQuoteId == plant.PlantsForQuoteId).Sum(x => x.QuantityToPick);
                }
                else
                {
                    plantOnPicklistQty = 0;
                }

                if (plant.Quantity > plantOnPicklistQty && plant.Active == true)
                {
                    plantsReturn.Add(new PlantsToReturn()
                    {
                        PlantForQuoteId = plant.PlantsForQuoteId,
                        PlantName = plant.PlantName,
                        FormSize = plant.FormSize,
                        AmountNeeded = (plant.Quantity - plantOnPicklistQty) ?? 0,
                        //Add in location and other stuff here

                        Active = plant.Active,
                    });
                }
                else
                {
                    //do nothing
                }
            }

            return plantsReturn;
        }

        // GET: api/picklist/plantsAlreadyOnPicklist?id={id} <---Pass in the quoteId here
        [Route("plantsAlreadyOnPicklist")]
        public List<PlantsOnSalesOrder> GetPlantsThatExistOnPicklists(int id)
        {
            var quote = db.Quotes.FirstOrDefault(q => q.QuoteId == id);

            var plantsFromQuote = quote.PlantsForQuotes.Select(item => new PlantsOnSalesOrder //Get all the plants that are currently for that sales order
            {
                PlantForQuoteId = item.PlantsForQuoteId,
                Quantity = item.Quantity ?? 0,
                PlantName = item.PlantName,
                FormSize = item.FormSize,
                Comment = item.Comment,
                Price = item.Price ?? 0,
                QuantityPicked = db.PlantsForPicklists.FirstOrDefault(x => x.PlantForQuoteId == item.PlantsForQuoteId) == null ? 0 : db.PlantsForPicklists.Where(x => x.PlantForQuoteId == item.PlantsForQuoteId).Sum(x => x.QuantityPicked),
                Active = item.Active,
            }).ToList();

            return plantsFromQuote;
        }


        public class PlantsOnSalesOrder
        {
            public int PlantForQuoteId { get; set; }
            public string Sku { get; set; }
            public string PlantName { get; set; }
            public string FormSize { get; set; }
            public string Comment { get; set; }
            public int Price { get; set; }
            public int Quantity { get; set; }
            public int QuantityPicked { get; set; }
            public bool Active { get; set; }
        }

        // POST: api/PickList
        [ResponseType(typeof(SelectedPickListDTO))]
        public HttpResponseMessage Post(HttpRequestMessage request, [FromBody] SelectedPickListDTO picklist)
        {
            if (!ModelState.IsValid)
            {
                return request.CreateResponse(HttpStatusCode.BadRequest, picklist);
            }

            try
            {
                var newPickList = new Picklist
                {
                    QuoteId = picklist.QuoteId,
                    DispatchDate = DateTime.ParseExact(picklist.DispatchDate, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                    DeliveryAddress = picklist.DeliveryAddress,
                    DeliveryNeeded = picklist.DeliveryNeeded,
                    IsPicked = picklist.IsPicked,
                    IsAllocated = picklist.IsAllocated,
                    IsDelivered = picklist.IsDelivered,
                    Comment = picklist.Comment,
                    EstimatedDelivery = picklist.EstimatedDelivery,
                    Active = true  //Default to true as initial save
                };

                db.Picklists.Add(newPickList);
                db.SaveChanges();
                db.Entry(newPickList).Reload();

                var pickListId = newPickList.PicklistId;
                foreach (var plant in picklist.PickListPlants)
                {

                    var thisPlant = new PlantsForPicklist
                    {
                        PicklistId = pickListId,
                        PlantForQuoteId = plant.PlantForQuoteId,
                        BatchId = plant.BatchId,
                        PlantName = plant.PlantName,
                        FormSize = plant.FormSize,
                        QuantityToPick = plant.QuantityToPick,
                        IsSubbed = plant.IsSubbed,
                        DispatchLocation = plant.DispatchLocation,
                        QuantityPicked = 0, //Default this to 0 because none will have been picked on creation??
                        Active = true
                    };
                    db.PlantsForPicklists.Add(thisPlant);
                }
                db.SaveChanges();


                return request.CreateResponse(HttpStatusCode.OK, picklist);
            }
            catch (Exception ex)
            {
                return request.CreateResponse(HttpStatusCode.BadRequest, ex);
            }
        }


        // PUT: api/picklist/delete?={id}
        [Route("delete")]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutPickListDelete(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (db.Picklists.Find(id) == null)
            {
                return BadRequest();
            }
            // get the object out of the db
            var pl = db.Picklists.Find(id);

            //Set the current value of active to be false
            pl.Active = false;

            db.Entry(pl).State = System.Data.Entity.EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PickListExists(id))
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

        private bool PickListExists(int id)
        {
            return db.Picklists.Count(e => e.PicklistId == id) > 0;
        }

        // PUT: api/picklist/pickItems?={id}
        [Route("pickItems")]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutPickItems(HttpRequestMessage request, SelectedPickListDTO picklistItems)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!PickListExists(picklistItems.PicklistId))
            {
                return NotFound();
            }

            var currentPicklist = db.Picklists.Where(x => x.PicklistId == picklistItems.PicklistId).FirstOrDefault();
            //Will always default to true, meaning that all the plants on the picklist have been picked
            //If not it will be set to false in foreach statement if a plant has only been partially picked
            bool pickedState = true;
            foreach (var item in picklistItems.PickListPlants)
            {
                //Saving the amount that has been picked to each plant on the current picklist
                var plantToChange = db.PlantsForPicklists.Where(x => x.PlantForPickListId == item.PlantForPicklistId).FirstOrDefault();
                if (plantToChange != null)
                {
                    //int dbQuantityPicked = plantToChange.QuantityPicked
                    //int calcQuantityPicked = plantToChange.QuantityPicked += item.QuantityPicked;
                    plantToChange.QuantityPicked += item.QuantityPicked; //Need to change this so it doesnt add
                    //plantToChange.CurrentQuantityPicked = item.QuantityPicked;
                    if (plantToChange.QuantityPicked < plantToChange.QuantityToPick)
                    {
                        pickedState = false;
                    }
                }
                else
                {
                    return StatusCode(HttpStatusCode.BadRequest);
                }

                db.Entry(plantToChange).State = EntityState.Modified;
            }
            currentPicklist.IsAllocated = false;
            currentPicklist.IsPicked = pickedState;

            db.Entry(currentPicklist).State = EntityState.Modified;


            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PickListExists(picklistItems.PicklistId))
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

        //private bool PickListItemExists(int id)
        //{
        //    return db.Picklists.Count(e => e.PicklistId == id) > 0;
        //}
    }
}



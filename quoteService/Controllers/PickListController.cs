﻿using System;
using System.Collections.Generic;
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
            var justOne = db.Picklists.SingleOrDefault(q => q.PicklistId == id);

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
                SubbedFor = item.SubbedFor,
                IsSubbed = item.isSubbed,
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

            foreach(var plant in plantsOnQuote)
            {
                int plantOnPicklistQty;
                //var exists = db.PlantsForPicklists.Where(x => x.PlantForQuoteId == plant.PlantsForQuoteId).FirstOrDefault();
                var exists = db.PlantsForPicklists.Any(x => x.PlantForQuoteId == plant.PlantsForQuoteId);
                if (exists == true)
                {
                    plantOnPicklistQty = db.PlantsForPicklists.Where(x => x.PlantForQuoteId == plant.PlantsForQuoteId).Sum(x => x.QuantityToPick);
                } else
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
                } else
                {
                    //do nothing
                }
            }

            return plantsReturn;
        }

        // POST: api/PickList
        [ResponseType(typeof(SelectedPickListDTO))]
        public HttpResponseMessage Post(HttpRequestMessage request, [FromBody]SelectedPickListDTO picklist)
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
                        SubbedFor = plant.SubbedFor,
                        isSubbed = plant.IsSubbed,
                        DispatchLocation = plant.DispatchLocation,
                        QuantityPicked = 0, //Can I just default this to 0 because none will have been picked on creation??
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

        // PUT: api/PickList/5
        public void Put(int id, [FromBody]string value)
        {
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
    }
}

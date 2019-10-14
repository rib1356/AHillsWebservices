using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using PickListService.DTO;

namespace PickListService.Controllers
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
                DipatchDate = item.DispatchDate,
                DeliveryAddress = item.DeliveryAddress,
                DeliveryNeeded = item.DeliveryNeeded,
                IsPicked = item.IsPicked,
                IsPacked = item.IsPicked,
                IsDelivered = item.IsDelivered,
                Comment = item.Comment,
                EsimatedDelivery = item.EstimatedDelivery, //Boolean because its either Estimated Or Exact Delivery Date
                Active = item.Active,
            }).AsEnumerable();

            return picklistToRet;
        }

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
                PlantName = item.PlantName,
                FormSize = item.FormSize,
                QuantityToPick = item.QuantityToPick,
                SubbedFor = item.SubbedFor,
                IsSubbed = item.isSubbed,
                DispatchLocation = item.DispatchLocation,
                Active = item.Active,
            });


            var dto = new DTO.SelectedPickListDTO
            {
                PicklistId = justOne.PicklistId,
                QuoteId = justOne.QuoteId,
                DipatchDate = justOne.DispatchDate,
                DeliveryAddress = justOne.DeliveryAddress,
                DeliveryNeeded = justOne.DeliveryNeeded,
                IsPicked = justOne.IsPicked,
                IsPacked = justOne.IsPacked,
                IsDelivered = justOne.IsDelivered,
                Active = justOne.Active,
                Comment = justOne.Comment,
                EsimatedDelivery = justOne.EstimatedDelivery, //Boolean because its either Estimated Or Exact Delivery Date
                PickListPlants = plantsForPicklist,
            };

            return dto;
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
                    DispatchDate = picklist.DipatchDate,
                    DeliveryAddress = picklist.DeliveryAddress,
                    DeliveryNeeded = picklist.DeliveryNeeded,
                    IsPicked = picklist.IsPicked,
                    IsPacked = picklist.IsPacked,
                    IsDelivered = picklist.IsPacked,
                    Comment = picklist.Comment,
                    EstimatedDelivery = picklist.EsimatedDelivery,
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
                        PlantName = plant.PlantName,
                        FormSize = plant.FormSize,
                        QuantityToPick = plant.QuantityToPick,
                        SubbedFor = plant.SubbedFor,
                        isSubbed = plant.IsSubbed,
                        DispatchLocation = plant.DispatchLocation,
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

        // DELETE: api/PickList/5
        public void Delete(int id)
        {
        }
    }
}

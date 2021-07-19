using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using BatchModel;
using BatchService.Models;

namespace BatchService.Controllers
{
    [EnableCors("*", "*", "*")]
    public class BatchesController : ApiController
    {
        private HillsStockEntities db = new HillsStockEntities();

        //A test comment1
        [Route("api/Batches/All")]
        // GET: api/Batches
        /// Send a collection of active BatchItemDTO's 
        /// Currently purchase Price does not exist in domain
       


        public IQueryable<BatchItemDTO> GetAllBatches()
        {
            var all = db.Batches.Where(b => b.Active == true);
            List<BatchItemDTO> DTO = new List<BatchItemDTO>();
            foreach(var item in all)
            {
                var b = new BatchItemDTO {
                    Id = item.Id,
                    Sku = item.Sku,
                    Name = item.Name,
                    Location = item.Location,
                    FormSize = item.FormSize,
                    FormSizeCode = item.FormSizeCode,
                    PurchasePrice = ConvertMe(item.BuyPrice),
                    WholesalePrice = item.WholesalePrice,
                    Comment = item.Comment

                };
                DTO.Add(b);


        };
            return DTO.AsQueryable();
        }

        private int? TransformToInt(decimal? v)
        {
            throw new NotImplementedException();
        }

        private int? ConvertMe(decimal? buyPrice)
        {
            if (buyPrice != null)
            {
               return  Convert.ToInt32(buyPrice);
            }
            return null;
        }

        //A test comment1
        [Route("api/Batches/Luke")]
        // GET: api/Batches
        /// Send a collection of active BatchItemDTO's 
        /// Currently purchase Price does not exist in domain
        public BatchListDTO GetAllBatchesForLuke()
        {
            var all = db.Batches;
            int skip = 0;
            int take = 50;
            var queryString = System.Web.HttpContext.Current.Request.QueryString;
            if (!(string.IsNullOrEmpty(queryString["$skip"])))
            {
                skip = Convert.ToInt32(queryString["$skip"]);  //paging 
            }
            if (!(string.IsNullOrEmpty(queryString["$top"])))
            {
                take = Convert.ToInt32(queryString["$top"]);  //paging 
            }



            //take = Convert.ToInt32(queryString["top"]); //paging 


            var result = all.Select(item => new BatchItemDTO
            {
                Id = item.Id,
                Sku = item.Sku,
                Name = item.Name,
                Location = item.Location,
                FormSize = item.FormSize,
                PurchasePrice = item.WholesalePrice,
                WholesalePrice = item.WholesalePrice,


            });
            BatchListDTO output = new BatchListDTO();

            //Items = data.Skip(skip).Take(take), 
            //output.Items = result.OrderBy(x => x.Id).Skip(skip).Take(take).ToList();
            output.Items = result.ToList();
            // output.Count = output.Items.Count();
            output.Count = result.Count();

            return output;
        }

        



        [Route("api/Batches/All/{Sku}")]
        // GET: api/Batches
        /// Send a collection of active BatchItemDTO's 
        /// Currently purchase Price does not exist in domain
        public List<BatchFormDTO> GetBatchDTO(string sku)
        {
            List<Batch> batch = db.Batches.Where(b => b.Sku == sku).ToList();
            if (batch == null)
            {
                return null;
            }
            else
            {
                var result = batch.Select(b=> new BatchFormDTO
                {
                    Sku = b.Sku,
                    Name = b.Name,
                    FormSize = b.FormSize,
                    WholesalePrice = Convert.ToDecimal(b.WholesalePrice)/100

            }).ToList();
                return result;
            }
            

        }

        private int getGQ(Batch b)
        { 
            var gq = 0;
                if (b.GrowingQuantity.HasValue && b.GrowingQuantity > 0)
                {
                    gq = Convert.ToInt32(b.GrowingQuantity);
                }
                else
                {
                    gq = 0;
                }
            return gq;
         }


        [Route("api/Batches/All/{id:int}")]
        // GET: api/Batches
        /// Send a collection of active BatchItemDTO's 
        /// Currently purchase Price does not exist in domain
        public BatchItemDTO GetBatchDTO(int id)
        {
            Batch batch = db.Batches.Find(id);
            if (batch == null)
            {
                return null;
            }
            else
            {
                /// the db has growing quantity as nullable so we need to check and cast the value;
                var gq = 0;
                if (batch.GrowingQuantity.HasValue && batch.GrowingQuantity > 0)
                {
                    gq = Convert.ToInt32(batch.GrowingQuantity);
                }
                else
                {
                    gq = 0;
                }

                return new BatchItemDTO
                {
                    Id = batch.Id,
                    Sku = batch.Sku,
                    Name = batch.Name,
                    FormSize = batch.FormSize,
                    PurchasePrice = -1,
                    Quantity = batch.Quantity,
                    GrowingQuantity = gq,
                    Location = batch.Location,
                    WholesalePrice = batch.WholesalePrice
                };
            }

        }

        //Get: api/batches
        public List<Batch> GetBatches()
        {
            //var batches = db.Batches.Where(x => x.Location != "PB").ToList();
            List<Batch> pBBatches = db.Batches.Where(b => b.Location == "PB").Take(7500).ToList();
            List<Batch> LocalBatches = db.Batches.Where(b => b.Location != "PB").Take(7500).ToList();
            //var testBatches = db.Batches.ToList();
            pBBatches.AddRange(LocalBatches);
            return pBBatches;
        }

        [Route("api/getBatchCount")]
        public int GetBatchCount()
        {
            var batchCount = db.Batches.Where(x => x.Active).Count();
            return batchCount;
        }

        // GET: api/Batches/5
        [ResponseType(typeof(Batch))]
        public IHttpActionResult GetBatch(int id)
        {
            Batch batch = db.Batches.Find(id);
            if (batch == null)
            {
                return NotFound();
            }

            return Ok(batch);
        }

        // PUT: api/Batches/id
        [ResponseType(typeof(void))]
        public HttpResponseMessage PutBatch(HttpRequestMessage request, int id, Batch batch)
        {
            if (!ModelState.IsValid)
            {
                return request.CreateResponse(HttpStatusCode.BadRequest, batch);
            }

            // get the object out of the db
            var b = db.Batches.Find(id);

            //Check the incoming batch, if the value is not null, keep it the same as the one from the db (var b)
            //Else set the value in the database to be equal to the one passed in
            if ( batch.Sku != null )
            {
                b.Sku = batch.Sku;
            }
            if ( batch.Name != null )
            {
                b.Name = batch.Name;
            }
            if ( batch.FormSize != null)
            {
                b.FormSize = batch.FormSize;
            }
            if ( batch.Location != null )
            {
                b.Location = batch.Location;
            }
            if ( batch.Quantity >= 0) //Check this if errors
            {
                b.Quantity = batch.Quantity;
            }
            if (batch.WholesalePrice != null)
            {
                b.WholesalePrice = batch.WholesalePrice;
            }
            if (batch.ImageExists != null)
            {
                b.ImageExists = batch.ImageExists;
            }
            if (batch.GrowingQuantity != null)
            {
                b.GrowingQuantity = batch.GrowingQuantity;
            }
            if (batch.AllocatedQuantity != null)
            {
                b.AllocatedQuantity = batch.AllocatedQuantity;
            }
            if(batch.DateStamp != null)
            {
                b.DateStamp = batch.DateStamp;
            }
            if (batch.Comment != null)
            {
                b.Comment = batch.Comment;
            }
            if (batch.Active != null) //What to do about this
            {
                b.Active = batch.Active;
            }

            if (id != batch.Id)
            {
                return request.CreateResponse(HttpStatusCode.BadRequest, batch);
            }

            db.Entry(b).State = EntityState.Modified;

         

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BatchExists(id))
                {
                    return request.CreateResponse(HttpStatusCode.NotFound, batch);
                }
                else
                {
                    throw;
                }
            }

            return request.CreateResponse(HttpStatusCode.OK, batch);
        }

        [Route("Batches/edit")]
        // PUT: api/Batches/edit
        [ResponseType(typeof(void))]
        public HttpResponseMessage PutBatchPrices(HttpRequestMessage request, IEnumerable<Models.BatchPriceDTO> BatchPrices)
        {
            if (!ModelState.IsValid)
            {
                return request.CreateResponse(HttpStatusCode.BadRequest, BatchPrices);
            }

            foreach (var batchPrice in BatchPrices) //Loop through the array of objects that hold BatchId, Price
            {
                var batchToChange = db.Batches.SingleOrDefault(b => b.Id == batchPrice.BatchId); //Get the batch by ID
                if (batchToChange == null)
                {
                    return request.CreateResponse(HttpStatusCode.NotFound, BatchPrices); //If its not null
                }

                batchToChange.WholesalePrice = batchPrice.Price; //Change the batchPrice to the one that is coming in

                db.Entry(batchToChange).State = EntityState.Modified; //Update the model
            }

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                foreach (var batchPrice in BatchPrices) //Loop through and to check if the batches exist
                {
                    if (!BatchExists(batchPrice.BatchId))
                    {
                        return request.CreateResponse(HttpStatusCode.NotFound, BatchPrices);
                    }
                }

                throw;
            }
            return request.CreateResponse(HttpStatusCode.OK, BatchPrices);
        }

        [Route("api/Batches/location")]
        // PUT: api/Batches/edit
        [ResponseType(typeof(void))]
        public HttpResponseMessage PutBatchLocation(HttpRequestMessage request, Models.BatchLocationDTO batch)
        {
            if (!ModelState.IsValid)
            {
                return request.CreateResponse(HttpStatusCode.BadRequest, batch);
            }


                var batchToChange = db.Batches.SingleOrDefault(b => b.Id == batch.BatchId); //Get the batch by ID
                if (batchToChange == null)
                {
                    return request.CreateResponse(HttpStatusCode.NotFound, batch); //If its not null
                }

                
                batchToChange.Location = batch.Location; //Change the batchQ to the one that is coming in
                if (batch.Quantity > 0)
                {
                    batchToChange.Quantity = batch.Quantity; //Change the batchQ to the one that is coming in
                    batchToChange.GrowingQuantity = 0;
                }
                else
                {
                    batchToChange.GrowingQuantity = batch.GrowingQuantity;
                    batchToChange.Quantity = 0;
                }

                db.Entry(batchToChange).State = EntityState.Modified; //Update the model


            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
               

                throw;
            }
            return request.CreateResponse(HttpStatusCode.OK, batch);
        }

        // POST: api/Batches
        [ResponseType(typeof(Batch))]
        public IHttpActionResult PostBatch(Batch batch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Batches.Add(batch);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = batch.Id }, batch);
        }

        [HttpPost]
        [Route("api/Batches/New",Name="stockTakePost")]
        [ResponseType(typeof(Batch))]
        public IHttpActionResult PostNewBatch([FromBody]Models.BatchItemDTO batch)
        {
            Batch newBatch = new Batch();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existing = db.Batches.Find(batch.Id);
            if (existing != null)
            {
                newBatch.BuyPrice = existing.BuyPrice;
                newBatch.WholesalePrice = existing.WholesalePrice;
            }
            else
            {
                newBatch.BuyPrice = 0;
                newBatch.WholesalePrice = 0;
            }

            newBatch.Active = true;
            newBatch.Name = existing.Name;
            newBatch.AllocatedQuantity = 0;
            newBatch.DateStamp = DateTime.Now;
            newBatch.FormSize = batch.FormSize;
            newBatch.GrowingQuantity = batch.GrowingQuantity;
            newBatch.ImageExists = false;
            newBatch.Location = batch.Location;
            newBatch.FormSize = batch.FormSize;
            newBatch.Quantity = batch.Quantity;
            newBatch.Sku = batch.Sku;
            db.Batches.Add(newBatch);
            db.SaveChanges();


            //var response = Request.CreateResponse(HttpStatusCode.Created);

            //// Generate a link to the new book and set the Location header in the response.
            string uri = Url.Link("stockTakePost", new { id = newBatch.Id });
            //response.Headers.Location = new Uri(uri);
            //return response;

            //  return CreatedAtRoute("DefaultApi", new { id = newBatch.Id }, newBatch);
            return CreatedAtRoute("stockTakePost", new { id = newBatch.Id }, newBatch);
        }

        // DELETE: api/Batches/5
        [ResponseType(typeof(Batch))]
        public IHttpActionResult DeleteBatch(int id)
        {
            Batch batch = db.Batches.Find(id);
            if (batch == null)
            {
                return NotFound();
            }

            db.Batches.Remove(batch);
            db.SaveChanges();

            return Ok(batch);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool BatchExists(int id)
        {
            return db.Batches.Count(e => e.Id == id) > 0;
        }
    }
}
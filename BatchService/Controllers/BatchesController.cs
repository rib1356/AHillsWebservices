﻿using System;
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

namespace BatchService.Controllers
{
    [EnableCors("*", "*", "*")]
    public class BatchesController : ApiController
    {
        private HillsStockEntities db = new HillsStockEntities();


       
        // GET: api/Batches
        public IQueryable<Batch> GetBatches()
        {
            return db.Batches;
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
            if ( batch.Quantity != 0) //Check this if errors
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
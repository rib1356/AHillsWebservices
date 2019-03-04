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

        // PUT: api/Batches/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutBatch(int id, Batch batch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
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
            if (batch.Image != null)
            {
                b.Image = batch.Image;
            }
            if (batch.Active != null)
            {
                b.Active = batch.Active;
            }

            if (id != batch.Id)
            {
                return BadRequest();
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
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
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
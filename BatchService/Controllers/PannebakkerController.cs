using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using BatchService.Models;
using BatchModel;

namespace BatchService.Controllers
{
    public class PannebakkerController : ApiController
    {
        private HillsStockEntities db = new HillsStockEntities();

        // GET: api/Pannebakker
        public IQueryable<Batch> GetBatches()
        {
            return db.Batches;
        }

        // GET: api/Pannebakker/5
        //[ResponseType(typeof(Batch))]
        public List<Pannebakker> GetBatchesByName(string plantName)
        {
            //This is probably going to be a really slow way to search through the list
            //And the spelling will have to exactly match the name?
            //Probably worth making a "General" search
            var returnable = db.Pannebakkers.Where(x => x.Name.ToLower().Trim() == plantName.ToLower().Trim()).ToList();

            return returnable;

        }

        // PUT: api/Pannebakker/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutBatch(int id, Batch batch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != batch.Id)
            {
                return BadRequest();
            }

            db.Entry(batch).State = EntityState.Modified;

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

        // POST: api/Pannebakker
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

        // DELETE: api/Pannebakker/5
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
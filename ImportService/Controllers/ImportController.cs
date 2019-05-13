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
using ImportService.Models;

namespace ImportService.Controllers
{
    public class ImportController : ApiController
    {
        private HillsStockImportEntities db = new HillsStockImportEntities();

        // GET: api/Import
        public IQueryable<Pannebakker> GetPannebakkers()
        {
            return db.Pannebakkers;
        }

        // GET: api/Import/5
        [ResponseType(typeof(Pannebakker))]
        public IHttpActionResult GetPannebakker(int id)
        {
            Pannebakker pannebakker = db.Pannebakkers.Find(id);
            if (pannebakker == null)
            {
                return NotFound();
            }

            return Ok(pannebakker);
        }

        // PUT: api/Import/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutPannebakker(int id, Pannebakker pannebakker)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != pannebakker.Id)
            {
                return BadRequest();
            }

            db.Entry(pannebakker).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PannebakkerExists(id))
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

        // POST: api/Import
        [ResponseType(typeof(Pannebakker))]
        public IHttpActionResult PostPannebakker(Pannebakker pannebakker)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Pannebakkers.Add(pannebakker);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = pannebakker.Id }, pannebakker);
        }

        // DELETE: api/Import/5
        [ResponseType(typeof(Pannebakker))]
        public IHttpActionResult DeletePannebakker(int id)
        {
            Pannebakker pannebakker = db.Pannebakkers.Find(id);
            if (pannebakker == null)
            {
                return NotFound();
            }

            db.Pannebakkers.Remove(pannebakker);
            db.SaveChanges();

            return Ok(pannebakker);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PannebakkerExists(int id)
        {
            return db.Pannebakkers.Count(e => e.Id == id) > 0;
        }
    }
}
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
using PlantService.Models;
using PlantService.DTO;


namespace PlantService.Controllers
{
    [EnableCors("*", "*", "*")]
    public class PlantController : ApiController
    {
        private HillsStock1Entities db = new HillsStock1Entities();

        //GET: api/Plant
        //public IQueryable<PlantName> GetPlantNames()
        //{
        //    return db.PlantNames;
        //}
        public IEnumerable<PlantNameDTO> GetPlantNames()
        {

            var dto = db.PlantNames.Select(item => new DTO.PlantNameDTO
            {
                plantId = item.PlantId,
                plantName = item.Name,
                Sku = item.Sku
            }).AsEnumerable();

            return dto;
        }

       

        // GET: api/Plant    
        //public List<string> GetPlantNames()
        //{
        //    var all = db.PlantNames;
        //    HashSet<string> result = new HashSet<string>(all.Select(item => item.Name));
        //    return result.ToList();
        //}


        // GET: api/Plant/5
        [ResponseType(typeof(PlantName))]
        public IHttpActionResult GetPlantName(int id)
        {
            PlantName plantName = db.PlantNames.Find(id);
            if (plantName == null)
            {
                return NotFound();
            }

            return Ok(plantName);
        }

        // PUT: api/Plant/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutPlantName(int id, PlantName plantName)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != plantName.PlantId)
            {
                return BadRequest();
            }

            db.Entry(plantName).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlantNameExists(id))
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

        // POST: api/Plant
        [ResponseType(typeof(PlantName))]
        public IHttpActionResult PostPlantName(PlantName plantName)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.PlantNames.Add(plantName);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = plantName.PlantId }, plantName);
        }

        // DELETE: api/Plant/5
        [ResponseType(typeof(PlantName))]
        public IHttpActionResult DeletePlantName(int id)
        {
            PlantName plantName = db.PlantNames.Find(id);
            if (plantName == null)
            {
                return NotFound();
            }

            db.PlantNames.Remove(plantName);
            db.SaveChanges();

            return Ok(plantName);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PlantNameExists(int id)
        {
            return db.PlantNames.Count(e => e.PlantId == id) > 0;
        }
    }
}
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



namespace LocationService
{
    [EnableCors("*", "*", "*")]
    //[RoutePrefix("api/locations")] and[Route("main")]
    [RoutePrefix("api/locations")]

    public class LocationsController : ApiController
    {
        private HillsStock1Entities db = new HillsStock1Entities();

        // GET: api/Locations
        public IQueryable<Location> GetLocations()
        {
            return db.Locations;
        }

        // GET: api/Locations/5
        [ResponseType(typeof(Location))]
        public IHttpActionResult GetLocation(int id)
        {
            Location locations = db.Locations.Find(id);
            if (locations == null)
            {
                return NotFound();
            }

            return Ok(locations);
        }

        // GET : api/locations/main
        [Route("main")]
        public List<string> GetMainLocations()
        {
            var all = db.Locations;
            HashSet<string> result = new HashSet<string>(all.Select(item => item.MainLocation));
            return result.ToList();
        }

        // GET: api/Locations?main={main}
        [ResponseType(typeof(IEnumerable<Location>))]
        public IHttpActionResult GetSubLocation(string main)
        {
           IEnumerable<Location> locations = db.Locations.Where(m => m.MainLocation == main);
            if (locations == null || locations.Count() == 0)
            {
                return NotFound();
            }

            return Ok(locations);
        }

        // PUT: api/Locations/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutLocation(int id, Location location)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != location.LocationId)
            {
                return BadRequest();
            }

            db.Entry(location).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LocationExists(id))
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

        // POST: api/Locations
        [ResponseType(typeof(Location))]
        public IHttpActionResult PostLocation(Location location)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Locations.Add(location);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = location.LocationId }, location);
        }

        // DELETE: api/Locations/5
        [ResponseType(typeof(Location))]
        public IHttpActionResult DeleteLocation(int id)
        {
            Location location = db.Locations.Find(id);
            if (location == null)
            {
                return NotFound();
            }

            db.Locations.Remove(location);
            db.SaveChanges();

            return Ok(location);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool LocationExists(int id)
        {
            return db.Locations.Count(e => e.LocationId == id) > 0;
        }
    }
}
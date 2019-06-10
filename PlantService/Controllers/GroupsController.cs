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
using PlantService.DTO;
using PlantService.Models;

namespace PlantService.Controllers
{
    [EnableCors("*", "*", "*")]
    public class GroupsController : ApiController
    {
        private HillsStock1Entities db = new HillsStock1Entities();

        
        // GET: api/Groups
        public IEnumerable<GroupDTO> GetGroups()
        {

            var dto = db.Groups.Select(item => new DTO.GroupDTO
            {
                GroupId = item.GroupId,
                GroupDescription = item.Description
            }).AsEnumerable();
            return dto;
        }

        // GET: api/Groups/5
        [ResponseType(typeof(Group))]
        public IHttpActionResult GetGroup(int id)
        {
            Group group = db.Groups.Find(id);
            if (group == null)
            {
                return NotFound();
            }

            return Ok(group);
        }

        // PUT: api/Groups/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutGroup(int id, Group group)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != group.GroupId)
            {
                return BadRequest();
            }

            db.Entry(group).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GroupExists(id))
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

        // POST: api/Groups
         [ResponseType(typeof(NewPlantDTO))]
        public HttpResponseMessage PostPlant(HttpRequestMessage request, [FromBody]NewPlantDTO newPlant)
        {
            if (!ModelState.IsValid)
            {
                return request.CreateResponse(HttpStatusCode.BadRequest, newPlant);
            }

            try
            {
                var thisPlant = new PlantName();
                thisPlant.Name = newPlant.Name;
                thisPlant.Sku = newPlant.Sku;

                db.PlantNames.Add(thisPlant); //Add the plant to the database
                db.SaveChanges(); //Save the changes
                db.Entry(thisPlant).Reload(); //reload the entry so that we are able to get the plant id

                var plantId = thisPlant.PlantId; //this can then be used to assign to the plant groups
                foreach (var group in newPlant.GroupDetails)
                {
                    var thisPlantGroup = new PlantGroup();
                    thisPlantGroup.PlantId = plantId;
                    thisPlantGroup.GroupId = group.GroupId;
                    db.PlantGroups.Add(thisPlantGroup);
                }
                db.SaveChanges();
                return request.CreateResponse(HttpStatusCode.OK, newPlant);
            }
            catch (Exception ex)
            {
               return request.CreateResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        // DELETE: api/Groups/5
        [ResponseType(typeof(Group))]
        public IHttpActionResult DeleteGroup(int id)
        {
            Group group = db.Groups.Find(id);
            if (group == null)
            {
                return NotFound();
            }

            db.Groups.Remove(group);
            db.SaveChanges();

            return Ok(group);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool GroupExists(int id)
        {
            return db.Groups.Count(e => e.GroupId == id) > 0;
        }
    }
}
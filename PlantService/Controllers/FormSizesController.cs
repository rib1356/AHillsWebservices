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
    public class FormSizesController : ApiController
    {
        private HillsStock1Entities db = new HillsStock1Entities();

        // GET: api/FormSizes
        public IEnumerable<FormSizeDTO> GetFormSizes()
        {
          
            var dto = db.FormSizes.Select(item => new DTO.FormSizeDTO
            {
                id = item.FormSizeId,
                GroupId = item.GroupId,
                Age = item.Age,
                HeightSpread = item.HeightSpread,
                Girth = item.Girth,
                PotSize = item.PotSize ?? 0, //If PotSize is null set the value to = 0
                RootType = item.RootType,
                Description = item.Description,
                Active = item.Active
            }).AsEnumerable();

            return dto;
        }

        public IEnumerable<DTO.FormSizeDTO> GetFormSizes(string sku)
        {
            var plant = db.PlantNames.SingleOrDefault(p => p.Sku == sku);
            var plantgroup = plant.PlantGroups;
            List<Group> ourset = new List<Group>();
            foreach(var g in plantgroup)
            {
                var groups = db.Groups.Where(f => f.GroupId == g.GroupId);
                ourset.AddRange(groups);
            }

            List<FormSize> ourform = new List<FormSize>();
            foreach (var g in ourset)
            {
                var thisform = db.FormSizes.Where(f => f.GroupId == g.GroupId);
                ourform.AddRange(thisform);
            }
            // this.approved_by = planRec.approved_by ?? planRec.approved_by.toString();
            // this.approved_by = IsNullOrEmpty(planRec.approved_by) ? "" : planRec.approved_by.toString();
            var dto = ourform.Select(item => new DTO.FormSizeDTO
            {
                id = item.FormSizeId,
                GroupId = item.GroupId,
                Age = item.Age ?? "",
                HeightSpread = item.HeightSpread ?? "",
                Girth = item.Girth ?? "",
                PotSize = item.PotSize ?? 0, //If PotSize is null set the value to = 0
                RootType = item.RootType ?? "",
                Description = item.Description ?? "",
                Active = item.Active
            }).AsEnumerable();

            // var group = db.FormSizes
            return dto;
        }

        // GET: api/FormSizes/5
        [ResponseType(typeof(FormSize))]
        public IHttpActionResult GetFormSize(int id)
        {
            FormSize formSize = db.FormSizes.Find(id);
            if (formSize == null)
            {
                return NotFound();
            }

            return Ok(formSize);
        }

        // PUT: api/FormSizes/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutFormSize(int id, FormSize formSize)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != formSize.FormSizeId)
            {
                return BadRequest();
            }

            db.Entry(formSize).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FormSizeExists(id))
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

        // POST: api/FormSizes
        [ResponseType(typeof(FormSize))]
        public IHttpActionResult PostFormSize(FormSize formSize)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.FormSizes.Add(formSize);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = formSize.FormSizeId }, formSize);
        }

        // DELETE: api/FormSizes/5
        [ResponseType(typeof(FormSize))]
        public IHttpActionResult DeleteFormSize(int id)
        {
            FormSize formSize = db.FormSizes.Find(id);
            if (formSize == null)
            {
                return NotFound();
            }

            db.FormSizes.Remove(formSize);
            db.SaveChanges();

            return Ok(formSize);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool FormSizeExists(int id)
        {
            return db.FormSizes.Count(e => e.FormSizeId == id) > 0;
        }
    }
}
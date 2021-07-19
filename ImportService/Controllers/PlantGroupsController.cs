using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ImportModel;

namespace ImportService.Controllers
{
   


    public class PlantGroupsController : Controller
    {
        private ImportEntities db = new ImportEntities();

        // GET: PlantGroups
        public ActionResult Index()
        {
            var plantGroups = db.PlantGroups.Include(p => p.Group).Include(p => p.PlantName);
            return View(plantGroups.ToList());
        }


        // GET: PlantGroups/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PlantGroup plantGroup = db.PlantGroups.Find(id);
            if (plantGroup == null)
            {
                return HttpNotFound();
            }
            return View(plantGroup);
        }

        // GET: PlantGroups/Create
        public ActionResult Create()
        {
            ViewBag.GroupId = new SelectList(db.Groups, "GroupId", "Description");
            ViewBag.PlantId = new SelectList(db.PlantNames, "PlantId", "Sku");
            return View();
        }

        // POST: PlantGroups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PlantGroupId,PlantId,GroupId")] PlantGroup plantGroup)
        {
            if (ModelState.IsValid)
            {
                db.PlantGroups.Add(plantGroup);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.GroupId = new SelectList(db.Groups, "GroupId", "Description", plantGroup.GroupId);
            ViewBag.PlantId = new SelectList(db.PlantNames, "PlantId", "Sku", plantGroup.PlantId);
            return View(plantGroup);
        }

        // GET: PlantGroups/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PlantGroup plantGroup = db.PlantGroups.Find(id);
            if (plantGroup == null)
            {
                return HttpNotFound();
            }
            ViewBag.GroupId = new SelectList(db.Groups, "GroupId", "Description", plantGroup.GroupId);
            ViewBag.PlantId = new SelectList(db.PlantNames, "PlantId", "Sku", plantGroup.PlantId);
            return View(plantGroup);
        }

        // POST: PlantGroups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PlantGroupId,PlantId,GroupId")] PlantGroup plantGroup)
        {
            var found = db.PlantGroups.FirstOrDefault(pg => pg.GroupId == plantGroup.GroupId && pg.PlantId == plantGroup.PlantId);

            if (ModelState.IsValid && found == null)
            {
                db.Entry(plantGroup).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index","Groups");
            }
            ViewBag.GroupId = new SelectList(db.Groups, "GroupId", "Description", plantGroup.GroupId);
            ViewBag.PlantId = new SelectList(db.PlantNames, "PlantId", "Sku", plantGroup.PlantId);
            return View(plantGroup);
        }

        // GET: PlantGroups/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PlantGroup plantGroup = db.PlantGroups.Find(id);
            if (plantGroup == null)
            {
                return HttpNotFound();
            }
            return View(plantGroup);
        }

        // POST: PlantGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PlantGroup plantGroup = db.PlantGroups.Find(id);
            db.PlantGroups.Remove(plantGroup);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

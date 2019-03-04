using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PlantService.Models;

namespace PlantService.Controllers
{
    public class PlantNamesController : Controller
    {
        private HillsStock1Entities db = new HillsStock1Entities();

        // GET: PlantNames
        public ActionResult Index()
        {
            return View(db.PlantNames.ToList());
        }


        // GET: api/PlantNames
        public IQueryable<PlantName> GetPlantNames()
        {
            return db.PlantNames;
        }

        // GET: PlantNames/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PlantName plantName = db.PlantNames.Find(id);
            if (plantName == null)
            {
                return HttpNotFound();
            }
            return View(plantName);
        }

        // GET: PlantNames/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PlantNames/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PlantId,Sku,Name")] PlantName plantName)
        {
            if (ModelState.IsValid)
            {
                db.PlantNames.Add(plantName);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(plantName);
        }

        // GET: PlantNames/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PlantName plantName = db.PlantNames.Find(id);
            if (plantName == null)
            {
                return HttpNotFound();
            }
            return View(plantName);
        }

        // POST: PlantNames/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PlantId,Sku,Name")] PlantName plantName)
        {
            if (ModelState.IsValid)
            {
                db.Entry(plantName).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(plantName);
        }

        // GET: PlantNames/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PlantName plantName = db.PlantNames.Find(id);
            if (plantName == null)
            {
                return HttpNotFound();
            }
            return View(plantName);
        }

        // POST: PlantNames/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PlantName plantName = db.PlantNames.Find(id);
            db.PlantNames.Remove(plantName);
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

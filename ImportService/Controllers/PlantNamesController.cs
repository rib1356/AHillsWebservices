using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ImportModel;
using ImportService.Models;
using PagedList;

namespace ImportService.Controllers
{
    public class PlantNamesController : Controller
    {
        private ImportEntities db = new ImportEntities();

        // GET: PlantNames
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {

            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "nameD" : "";
            ViewBag.SkuSortParm = sortOrder == "sku" ? "skuD" : "sku";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            List<PlantName> vm = new List<PlantName>();
            var allPlantNames = db.PlantNames;

            if (!String.IsNullOrEmpty(searchString))
            {
                vm = allPlantNames.Where(s => s.Name.ToUpper().Contains(searchString.ToUpper())
                                       || s.Sku.ToUpper().Contains(searchString.ToUpper())).ToList();
            }
            else
            {
                vm = allPlantNames.ToList();
            }

            switch (sortOrder)
            {
                case "nameD":
                    vm = vm.OrderByDescending(s => s.Name).ToList();
                    break;
                case "name":
                    vm = vm.OrderBy(s => s.Name).ToList();
                    break;
                case "sku":
                    vm = vm.OrderBy(s => s.Sku).ToList();
                    break;
                case "skuD":
                    vm = vm.OrderByDescending(s => s.Sku).ToList();
                    break;
                default:
                    vm = vm.OrderBy(s => s.Name).ToList();
                    break;
            }

            int pageSize = 20;
            int pageNumber = (page ?? 1);
            return View(vm.ToPagedList(pageNumber, pageSize));
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
        public ActionResult Create([Bind(Include = "Id,Sku,Name")] PlantName plantName)
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

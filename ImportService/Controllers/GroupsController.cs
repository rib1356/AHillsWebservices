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

    public class GroupPlantVM
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public List<PlantName> GroupPlants { get; set; }
    }

    public class GroupsController : Controller
    {
        private ImportEntities db = new ImportEntities();

        // GET: Groups
        public ActionResult Index()
        {
            return View(db.Groups.ToList());
        }

        // GET: Groups/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Group group = db.Groups.Find(id);
            List<PlantName> plantNames = new List<PlantName>();
            var PlantsInGroup = group.PlantGroups.Where(p => p.GroupId == id);
            foreach(var p in PlantsInGroup)
            {
                PlantName pn = new PlantName();
                pn.PlantId = p.PlantId;
                pn.Sku = p.PlantName.Sku;
                pn.Name = p.PlantName.Name;
                plantNames.Add(pn);
            }
            var vm = new GroupPlantVM();
            vm.GroupId = group.GroupId;
            vm.GroupName = group.Description;
            vm.GroupPlants = plantNames;
            if (group == null)
            {
                return HttpNotFound();
            }
            return View(vm);
        }



        // GET: Groups/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Groups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "GroupId,Description")] Group group)
        {
            if (ModelState.IsValid)
            {
                db.Groups.Add(group);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(group);
        }

        // GET: Groups/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Group group = db.Groups.Find(id);
            if (group == null)
            {
                return HttpNotFound();
            }
            return View(group);
        }

        // POST: Groups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "GroupId,Description")] Group group)
        {
            if (ModelState.IsValid)
            {
                db.Entry(group).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(group);
        }

        public ActionResult Remove(int? id,int?gid)
        {
            if (id == null | gid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PlantGroup plantGroup = db.PlantGroups.SingleOrDefault(p => p.GroupId == gid && p.PlantId == id);
            if (plantGroup == null)
            {
                return RedirectToAction("Index");
            }
          
            return View(plantGroup);
        }

        [HttpPost, ActionName("Remove")]
        public ActionResult RemoveConfirm(int? PlantGroupId)
        {
            PlantGroup group = db.PlantGroups.Find(PlantGroupId);
            db.PlantGroups.Remove(group);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Groups/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Group group = db.Groups.Find(id);
            if (group == null)
            {
                return HttpNotFound();
            }
            var plantsInGroup = group.PlantGroups.Count();
            if (plantsInGroup > 0)
            {
                return RedirectToAction("Details", new { id = group.GroupId });
            }
            return View(group);
        }

        // POST: Groups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Group group = db.Groups.Find(id);
            db.Groups.Remove(group);
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

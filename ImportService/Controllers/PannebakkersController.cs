using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ImportService.DTO;
using ImportService.Models;

namespace ImportService.Controllers
{
    public class PannebakkersController : Controller
    {
        private HillsStockImportEntities db = new HillsStockImportEntities();

        // GET: Pannebakkers
        //public ActionResult Index()
        //{
        //    return View(db.Pannebakkers.ToList());
        //}

        public ActionResult Index()
        {
            var PBbatches = new List<DTO.PbDTO>().AsEnumerable();
            // dear service can i have the batches please
            PBbatches = ServiceLayer.PbService.GetPbItems();
            // transform the services into a viewModel
            IEnumerable<DTO.PbVM> VM = buildVM(PBbatches);

            return View(VM);
        }

        private IEnumerable<PbVM> buildVM(IEnumerable<PbDTO> pBbatches)
        {
            return pBbatches.Select(b => new DTO.PbVM
            {
                PbId = b.PbId,
                BatchId = b.BatchId,
                Sku = b.Sku,
                Name = b.Name,
                FormSizeCode = b.FormSizeCode,
                Price = b.Price
            }).AsEnumerable();
        }

        // GET: Pannebakkers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pannebakker pannebakker = db.Pannebakkers.Find(id);
            if (pannebakker == null)
            {
                return HttpNotFound();
            }
            return View(pannebakker);
        }

        // GET: Pannebakkers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Pannebakkers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Sku,FormSizeCode,Name,FormSize,Price,RootBall")] Pannebakker pannebakker)
        {
            if (ModelState.IsValid)
            {
                db.Pannebakkers.Add(pannebakker);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(pannebakker);
        }

        // GET: Pannebakkers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pannebakker pannebakker = db.Pannebakkers.Find(id);
            if (pannebakker == null)
            {
                return HttpNotFound();
            }
            return View(pannebakker);
        }

        // POST: Pannebakkers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Sku,FormSizeCode,Name,FormSize,Price,RootBall")] Pannebakker pannebakker)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pannebakker).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(pannebakker);
        }

        // GET: Pannebakkers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pannebakker pannebakker = db.Pannebakkers.Find(id);
            if (pannebakker == null)
            {
                return HttpNotFound();
            }
            return View(pannebakker);
        }

        // POST: Pannebakkers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Pannebakker pannebakker = db.Pannebakkers.Find(id);
            db.Pannebakkers.Remove(pannebakker);
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

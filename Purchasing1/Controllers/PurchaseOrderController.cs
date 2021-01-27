using Purchasing1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Purchasing1.Controllers
{
    public class PurchaseOrderController : Controller
    {
        // GET: PurchaseOrder
        private HillsStockPEntities db = new HillsStockPEntities();
        public ActionResult Index(int id)
        {
            var pForq = db.PlantsForQuote.Where(p => p.QuoteId == id).ToList();
            var all = db.PlantsForPicklist.Where(p => p.NeedsPurchasing == true).ToList();
            List<PlantsForPicklist> pl = new List<PlantsForPicklist>();
            foreach(var p in all)
            {
               if(pForq.Any(r => r.PlantsForQuoteId == p.PlantForQuoteId))
                {
                    pl.Add(p);
                }
            }
            ViewBag.dataSource = pl;
            return View();
        }
    }
}
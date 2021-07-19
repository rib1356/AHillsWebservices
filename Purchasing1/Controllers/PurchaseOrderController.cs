
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

            List<PurchaseOrderItemVM> listVm = new List<PurchaseOrderItemVM>();
            foreach(var p in pl)
            {
                PurchaseOrderItemVM poitem = new PurchaseOrderItemVM();
                poitem.PlantName = p.PlantName;
                poitem.FormSize = p.FormSize;
                poitem.QuantityRequied = p.QuantityToPick;
                poitem.BatchUnitPrice = GetPrice(p);
                listVm.Add(poitem);
            }




            
            return View();
        }

        private Decimal GetPrice(PlantsForPicklist p)
        {
            Batch batch = db.Batch.Find(p.BatchId);
            return Convert.ToDecimal(batch.BuyPrice / 100);
        }
    }
}
using BatchService.Models;
using ImportRep;
using ImportService.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ImportService.Controllers
{
    public class FSPriceController : Controller
    {

        private IImportRepository db;

        public FSPriceController()
        {
            this.db = new ImportRepository(new ImportModel.ImportEntities());
        }
        // GET: FSPrice
        public ActionResult Index()
        {
            var Allbatches = db.GetPBBatches();
            var unChanged = Allbatches.Where(b => b.BuyPrice == b.WholesalePrice);
            List<DTO.BatchListItemVM> vm = new List<DTO.BatchListItemVM>();
            vm = unChanged.Select(p => new DTO.BatchListItemVM
            {
                 BatchId = p.Id,
                  Sku = p.Sku,
                   Name = p.Name,
                    FormSize = p.FormSize
            }).ToList();
            return View(vm);
        }

 

        // GET: FSPrice/Details/5
        public ActionResult Calculate()
        {
            // empty batches object to fill soon
            //var batches = new List<DTO.BatchDTO>();
            //var VM = new List<DTO.BatchEditVM>();
            // dear service can i have the batches please
            var Allbatches = db.GetPBBatches();
            var unChanged = Allbatches.Where(b => b.Comment == null);
            List<ImportModel.Batch> updateList = new List<ImportModel.Batch>();
            foreach (var b in unChanged)
            {
                // lets build a model we can edit
                /// get price datavar
                /// 
               if ( b.Id == 6854)
                {
                    var found = true; 
                }
                int? WholeSalePrice = b.WholesalePrice;
                PriceItemDTO batchWithPrice = PriceService.GetUnitPrice(b.FormSize, b.FormSizeCode);
                if ( batchWithPrice == null  )
                {

                    //DTO.BatchEditVM vm = new DTO.BatchEditVM();
                    //vm.BatchId = b.Id;
                    //vm.Sku = b.Sku;
                    //vm.Name = b.Name;
                    //vm.FormSize = b.FormSize;
                    //vm.FormSizeCode = b.FormSizeCode;
                    //vm.formType = "Dont Know";
                    //vm.PriceRule = "No Price Band";
                    //vm.maxPrice = 0;
                    //vm.minPrice = Convert.ToInt32(b.WholesalePrice)/100;
                    //VM.Add(vm);
                    //    var max = batchWithPrice.MaxUnitValue * 100;
                    //    var min = batchWithPrice.MinUnitValue * 100;
                    //    if (b.Price < min)
                    //    {
                    //        WholeSalePrice = Convert.ToInt32(min) + b.Price; 

                    //    }
                    //    if (b.Price > max)
                    //    {
                    //        WholeSalePrice = Convert.ToInt32(max) + b.Price;
                    //    }
                    //ImportService.DTO.BatchPriceDTO newPrice = new ImportService.DTO.BatchPriceDTO();
                    //newPrice.BatchId = b.Id;
                    //newPrice.Price = Convert.ToInt32(WholeSalePrice);

                }
                else
                {
                    // nothing yet
                    var newPrice = PriceService.CalCapPrice(b);
                    b.WholesalePrice = Convert.ToInt32(newPrice);
                    updateList.Add(b);
                }
            }
            db.BatchUpdate(updateList);
            return RedirectToAction("Index");
        }

        // GET: FSPrice/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: FSPrice/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: FSPrice/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: FSPrice/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: FSPrice/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: FSPrice/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}

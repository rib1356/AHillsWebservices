using BatchService.Models;
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
        // GET: FSPrice
        public ActionResult Index()
        {
            return View();
        }

 

        // GET: FSPrice/Details/5
        public ActionResult Calculate()
        {
            // empty batches object to fill soon
            var batches = new List<DTO.BatchDTO>();
            var VM = new List<DTO.BatchEditVM>();
            // dear service can i have the batches please
            batches = ServiceLayer.BatchService.GetBatches().Where(b => b.Location == "PB").ToList();
            foreach(var b in batches)
            {
                // lets build a model we can edit
                /// get price datavar
                /// 
               if ( b.Id == 1012043)
                {
                    var found = true; 
                }
                int? WholeSalePrice = b.PurchasePrice;
                PriceItemDTO batchWithPrice = PriceService.GetUnitPrice(b.FormSize, b.FormSizeCode);
                if (batchWithPrice != null )
                {
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
                    DTO.BatchEditVM vm = new DTO.BatchEditVM();
                    vm.BatchId = b.Id;
                    vm.Sku = b.Sku;
                    vm.Name = b.Name;
                    vm.FormSize = b.FormSize;
                    vm.FormSizeCode = b.FormSizeCode;
                    vm.formType = "Dont Know";
                    vm.PriceRule = "No Price Band";
                    vm.maxPrice = 0;
                    vm.minPrice = 0;
                    VM.Add(vm);
                }
            }
            return View(VM);
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

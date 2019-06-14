using ImportService.DTO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace ImportService.Controllers
{
    public class readBatchController : Controller
    {
        // GET: readBatch
        public ActionResult Index()
        {
            // empty batches object to fill soon
            var batches = new List<DTO.BatchDTO>().AsEnumerable();
            // dear service can i have the batches please
            batches = ServiceLayer.BatchService.GetBatches();
            // transform the services into a viewModel
            IEnumerable<DTO.BatchVM> VM = buildVM(batches);

            return View(VM);


        }


        public ActionResult PB(int? id)
        {
            // empty batches object to fill soon
            var pb = new List<DTO.PbDTO>().AsEnumerable();
            var batchItem = ServiceLayer.BatchService.GetBatchItem((int)id);
            // dear service can i have the batches please
            pb = ServiceLayer.PbService.GetPbBatchItems((int)id);
            // transform the services into a viewModel
            BatchPBVM VM = new BatchPBVM();
            IEnumerable<DTO.PbVM> List = PbVM.buildVM(pb);
            VM.BatchItem = new BatchVM
            {
                BatchId = batchItem.Id,
                FormSize = batchItem.FormSize,
                Name = batchItem.Name,
                Quantity = batchItem.Quantity,
                Sku = batchItem.Sku,
                WholesalePrice = batchItem.WholesalePrice
            };
            VM.PbList = List;
            return View(VM);


        }




        /// <summary>
        /// Builds a VM
        /// </summary>
        /// <param name="batches"></param>
        /// <returns></returns>
        private static IEnumerable<DTO.BatchVM> buildVM(IEnumerable<DTO.BatchDTO> batches)
        {
            return batches.Select(b => new DTO.BatchVM
            {
                BatchId = b.Id,
                Sku = b.Sku,
                Name = b.Name,
                FormSize = b.FormSize,
                Quantity = b.Quantity,
                WholesalePrice = b.WholesalePrice
            }).AsEnumerable();
        }


 
    }
}
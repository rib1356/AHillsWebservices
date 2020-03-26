using BatchService.Models;
using ImportService.DTO;
using PagedList;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ImportService.Controllers
{
    public class readBatchController : Controller
    {
        // GET: readBatch
        //public ActionResult Index()
        //{
        //    // empty batches object to fill soon
        //    var batches = new List<DTO.BatchDTO>().AsEnumerable();
        //    // dear service can i have the batches please
        //    batches = ServiceLayer.BatchService.GetBatches();
        //    // transform the services into a viewModel
        //    IEnumerable<DTO.BatchVM> VM = buildVM(batches);
        //    return View(VM);
        //}

        public ActionResult Index(string sortOrder, string currentFilter, string searchString, string FormSearchstring,int? page, bool? hasPB , bool? hasLocal)
        {

            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "nameD" : "";
            ViewBag.SkuSortParm = sortOrder == "sku" ? "skuD" : "sku";
            bool hasPB_ = true;
            bool hasLocal_ = true;
            if (hasPB == false | hasPB == null)
            {
                hasPB_ = false;
            }
            if (hasLocal == false | hasLocal == null)
            {
                hasLocal_ = false;
            }

            ViewBag.hasPB = hasPB;
            ViewBag.hasLocal = hasLocal;

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            ViewBag.FormCurrentFilter = FormSearchstring;

            // empty batches object to fill soon
            var batches = new List<DTO.BatchDTO>();
            // dear service can i have the batches please
            batches = ServiceLayer.BatchService.GetBatches().ToList();
            // transform the services into a viewModel
            List<DTO.BatchVM> VM = buildVM(batches).ToList();

            

            if (!String.IsNullOrEmpty(searchString))
            {
                VM = VM.Where(s => s.Name.ToUpper().Contains(searchString.ToUpper())
                                       || s.Sku.ToUpper().Contains(searchString.ToUpper())).ToList();
            }
            else
            {
                VM = VM.ToList();
            }

            if (!String.IsNullOrEmpty(FormSearchstring))
            {
                string phrase = FormSearchstring;
                string[] words = phrase.Split(' ');

                foreach (var word in words)
                {
                    var formsize = word;
                    VM = VM.Where(s => s.FormSize.ToUpper().Contains(formsize.ToUpper())).ToList();
                }


                // s => s.FormSize.ToUpper().Contains(formsize.ToUpper())
            }
            else
            {
                VM = VM.Where(s => s.FormSize.ToUpper().Contains("RB")).ToList();
            }

            switch (sortOrder)
            {
                case "nameD":
                    VM = VM.OrderByDescending(s => s.Name).ToList();
                    break;
                case "name":
                    VM = VM.OrderBy(s => s.Name).ToList();
                    break;
                case "sku":
                    VM = VM.OrderBy(s => s.Sku).ToList();
                    break;
                case "skuD":
                    VM = VM.OrderByDescending(s => s.Sku).ToList();
                    break;
                default:
                    VM = VM.OrderBy(s => s.Name).ToList();
                    break;
            }

            VM = LocalOrPb(hasPB_, hasLocal_, VM);

            // !myString.Equals("-1")

            int pageSize = 20;
            int pageNumber = (page ?? 1);
            return View(VM.ToPagedList(pageNumber, pageSize));
        }

        private static List<BatchVM> LocalOrPb(bool hasPB, bool hasLocal, List<BatchVM> VM)
        {

            if ((hasLocal && hasPB) | (!hasPB && !hasLocal))
            {
                return VM;
            }
            if (hasPB)
            {
                 return VM.Where(f => f.Location.Equals("PB")).ToList();
            }

            if (hasLocal)
            {
                return VM.Where(f => f.Location != "PB").ToList();
            }

           

            return VM;
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

        public async Task<ActionResult> Edit(string id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var batch = ServiceLayer.BatchService.GetBatchItem(Convert.ToInt32(id));
             if (batch == null)
            {
                return HttpNotFound();
            }
            BatchVM vm = new BatchVM
            {
                BatchId = batch.Id,
                Sku = batch.Sku,
                Name = batch.Name,
                FormSize = batch.FormSize,
                Location = batch.Location,
                Quantity = batch.Quantity

            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(BatchVM batch)
        {
            if (ModelState.IsValid)
            {
                BatchService.Models.BatchLocationDTO batchDTO = new BatchLocationDTO
                {
                    BatchId = batch.BatchId,
                    Location = batch.Location,
                    Quantity = batch.Quantity
                };
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://ahillsbatchservice.azurewebsites.net/");
                    var response = await client.PutAsJsonAsync("api/Batches/location", batchDTO);
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Server error try after some time.");
                    }
                }
                return RedirectToAction("Index");
            }
            return View(batch);
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
                Location = b.Location,
                WholesalePrice = b.WholesalePrice
            }).AsEnumerable();
        }


 
    }
}
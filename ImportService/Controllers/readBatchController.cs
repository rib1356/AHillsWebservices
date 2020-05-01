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
using System.Web.Mvc;

namespace ImportService.Controllers
{
    public class readBatchController : Controller
    {


      

        public ActionResult Index(string sortOrder, string currentFilter, string searchString, string FormSearchstring,int? page, bool? hasPB , bool? hasLocal)
        {
            ServiceLayer.BatchService.GetLocations();
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
                                       || s.Sku.ToUpper().Contains(searchString.ToUpper())).OrderBy(p =>p.Sku).ThenBy(p => p.Name).ThenBy(p => p.FormSize).ToList();
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
                    VM = VM.Where(s => s.FormSize.ToUpper().Contains(formsize.ToUpper())).OrderBy(p => p.Name).ToList();
                }


                // s => s.FormSize.ToUpper().Contains(formsize.ToUpper())
            }
            //else
            //{
            //    VM = VM.Where(s => s.FormSize.ToUpper().Contains("RB")).ToList();
            //}

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
            
            // it is possible i get asked for a null item  
            if (id == null)
            {
                // if i do then say bugger off
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // so lets get the item we need
            var batch = ServiceLayer.BatchService.GetBatchItem(Convert.ToInt32(id));

            // this batch does not exists 
            if (batch == null)
            {
                // then say so
                return HttpNotFound();
            }


            // lets build a model we can edit

            BatchEditVM vm = new BatchEditVM();
            vm.BatchId = batch.Id;
            vm.Sku = batch.Sku;
            vm.Name = batch.Name;
            vm.FormSize = batch.FormSize;

            // if it is a PB item the empty the location field as this makes it easier to 
            // to use in the field - last location is also to help users having to update this field as they move aroud the nursery
            if (batch.Location == "PB")
            {
                // empty location field
                if (Session["LastLocation"] != null)
                {
                    vm.MainLocation = (String)Session["LastLocation"];
                }
                else
                {
                    vm.MainLocation = "";
                }

                // PB have no sublocations
                vm.SubLocation = "";
                vm.Quantity = 0;
                vm.wasPB = true;
                vm.forSale = true;
            }
            else
            {
                /// so its a local item this snippet splits the database location 'on space' into
                // main and sub location
                string fullLocation = batch.Location;
                string[] parts = fullLocation.Split(' ');
                vm.MainLocation = parts[0];
                // it is possible a sub location has not been defined 
                if (parts.ElementAtOrDefault(1) != null)
                {
                    vm.SubLocation = parts[1];
                }
                else
                {
                    vm.SubLocation = "";
                }
                ///////////////////////////////////

                /// Quantity if there is no quntity there should be a growing quantity
                /// if quantity is valid they are for sale otherwise they are just growing and not yet for sale
                if (batch.Quantity > 0)
                {
                    vm.Quantity = batch.Quantity;
                    vm.forSale = true;
                }
                else
                {
                    vm.Quantity = batch.GrowingQuantity;
                    vm.forSale = false;
                }
                vm.wasPB = false;
            }




            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(BatchEditVM batch)
        {
            var locations = ServiceLayer.BatchService.GetLocations();
            if (ModelState.IsValid)
            {
               
                
                var mainLocation = batch.MainLocation;
                var subLocation = batch.SubLocation;
                var mainexists = locations.Any(l => l.MainLocation == mainLocation);
                if (mainexists)
                {
                    var subexists = locations.Any(l => l.SubLocation == subLocation);
                    if (subexists)
                    {
                        // var subexists = locations.Any(l => l.SubLocation == subLocation);
                        var sublocations = locations.Where(l => l.MainLocation == mainLocation);
                        //  var sublocations = ServiceLayer.BatchService.GetSubLocations(mainLocation);
                        var subvalid = sublocations.Any(l => l.SubLocation == subLocation);
                        if (!(subvalid))
                        {
                            ModelState.AddModelError(string.Empty, "Sub Location Does NOT exist for this Location");
                            return View(batch);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Sub Location Does NOT exist");
                        return View(batch);
                    }

                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Main Location Does NOT exist");
                    return View(batch);
                }
               

                /// This starts to build the DTO 
                /// it collates the Location using a space seperator 
                /// if its for sale quantity is quantity if not quantity is 0
                /// if its not for sale growing quantity is quantity if not growing quantity is 0
                /// so either quantity has a value or growing quantity has a value > 0 never both
                BatchService.Models.BatchItemDTO batchDTO = new BatchItemDTO
                {
                    Id = batch.BatchId,
                    Sku = batch.Sku,
                    FormSize = batch.FormSize,
                    Name = batch.Name,
                    Location = mainLocation + " " + subLocation,
                    Quantity = (batch.forSale) ? batch.Quantity : 0,
                    GrowingQuantity = (!(batch.forSale)) ? batch.Quantity : 0,
                };


                 /// Two possible senarios
                 /// One the current item is a PB item : we add the new local stock item to the DB 
                 /// and keep the origional PB un touched
                 /// Two the current item is Local : we update the existing local stock item
                using (var client = new HttpClient())
                {

                    //  https://ahillsbatchservice.azurewebsites.net/
                    client.BaseAddress = new Uri("https://ahillsbatchservice.azurewebsites.net/");
                    //client.BaseAddress = new Uri("http://localhost:52009/");

                    if (batch.wasPB == true)
                    {
                        /// I am a PB so POST and ADD
                        var response = await client.PostAsJsonAsync("api/Batches/New", batchDTO);
                        if (response.IsSuccessStatusCode)
                        {
                            // this ensures the UI presents the last used location
                            Session["LastLocation"] = mainLocation;
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Server error try after some time.");
                        }
                        ///
                    }
                    else
                    {
                        /// I am local so PUT and UPDATE
                        BatchService.Models.BatchLocationDTO updateDTO = new BatchService.Models.BatchLocationDTO();
                        updateDTO.BatchId = batch.BatchId;
                        updateDTO.Location = batch.MainLocation + " " + batch.SubLocation;

                        updateDTO.Quantity = (batch.forSale) ? batch.Quantity : 0;
                        updateDTO.GrowingQuantity = (!(batch.forSale)) ? batch.Quantity : 0;

                        var response = await client.PutAsJsonAsync("api/Batches/location", updateDTO);
                        if (response.IsSuccessStatusCode)
                        {
                            // this ensures the UI presents the last used location
                            Session["LastLocation"] = mainLocation;
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Server error try after some time.");
                        }
                        ///
                        
                    }
                }
                Session["LastLocation"] = mainLocation;
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
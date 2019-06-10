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
            var batches = new List<DTO.BatchDTO>().AsEnumerable(); 
            HttpClient client = ApiClient();
            HttpResponseMessage response = client.GetAsync("api/Batches/All").Result;
            if (response.IsSuccessStatusCode)
            {
                batches = response.Content.ReadAsAsync<IEnumerable<DTO.BatchDTO>>().Result;
            }
            else
            {
                Debug.WriteLine("Index received a bad response from the web service.");
            }
            IEnumerable<DTO.BatchVM> VM = batches.Select(b => new DTO.BatchVM
            {
                 Sku = b.Sku,
                  Name = b.Name,
                   FormSize = b.FormSize,
                    Quantity = b.Quantity,
                     WholesalePrice = b.WholesalePrice
            }).AsEnumerable();
            return View(VM);


        }


        /// <summary>
        /// WE need to modify the System URi to use the deployed service
        /// </summary>
        /// <returns></returns>
        private static HttpClient ApiClient()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new System.Uri("http://localhost:52009/");
            client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
            return client;
        }
    }
}
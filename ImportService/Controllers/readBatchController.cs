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
            // build a client to chat to the web service
            HttpClient client = ApiClient();
            // dear service can i have the batches please
            batches = GetBatches(batches, client);
            // transform the services into a viewModel
            IEnumerable<DTO.BatchVM> VM = buildVM(batches);

            return View(VM);


        }

        /// <summary>
        /// ASk our Client for the batches please
        /// </summary>
        /// <param name="batches"></param>
        /// <param name="client"></param>
        /// <returns>IEnumerable<DTO.BatchDTO></returns>
        private static IEnumerable<DTO.BatchDTO> GetBatches(IEnumerable<DTO.BatchDTO> batches, HttpClient client)
        {
            HttpResponseMessage response = client.GetAsync("api/Batches/All").Result;
            if (response.IsSuccessStatusCode)
            {
                batches = response.Content.ReadAsAsync<IEnumerable<DTO.BatchDTO>>().Result;
            }
            else
            {
                Debug.WriteLine("Index received a bad response from the web service.");
            }

            return batches;
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
                Sku = b.Sku,
                Name = b.Name,
                FormSize = b.FormSize,
                Quantity = b.Quantity,
                WholesalePrice = b.WholesalePrice
            }).AsEnumerable();
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
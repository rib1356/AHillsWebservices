using ImportServiceCore.DTO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace ImportService.ServiceLayer
{


    public class PriceService
    {
        /// <summary>
        /// WE need to modify the System URi to use the deployed services
        /// </summary>
        /// <returns></returns>
        private static HttpClient ApiClient()
        {
            HttpClient client = new HttpClient();
            // http://localhost:52009/
            client.BaseAddress = new System.Uri("http://localhost:61628/");
            //client.BaseAddress = new System.Uri("https://priceservice.azurewebsites.net/");
            client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
            return client;
        }

        public PriceItemDTO GetPUnitPrice(string formSize)
        {
            HttpClient client = ApiClient();
            var request = "api/Values?form=" + formSize;
            HttpResponseMessage response = client.GetAsync(request).Result;
            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsAsync<PriceItemDTO>().Result;
            }
            else
            {
                Debug.WriteLine("Index received a bad response from the web service.");
                return null;
            }


        }


    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace PriceService.ServiceLayer
{

    public class BatchDTO
    {
        public int Id { get; set; }
        public string Sku { get; set; }
        public string Name { get; set; }
        public string FormSize { get; set; }
        public string Location { get; set; }
        public int Quantity { get; set; }
        public int GrowingQuantity { get; set; }
        public Nullable<int> BuyPrice { get; set; }

    }

    public class BatchService
    {
        private static HttpClient ApiClient()
        {
            HttpClient client = new HttpClient();
            // http://localhost:52009/
            // http://localhost:64383/api/Locations
            //client.BaseAddress = new System.Uri("http://localhost:64383/");
            client.BaseAddress = new System.Uri("https://ahillsbatchservice.azurewebsites.net/");
            client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
            return client;
        }

        public static BatchDTO GetBatchItem(int id)
        {
            HttpClient client = ApiClient();
            var request = "api/Batches/" + id.ToString();
            HttpResponseMessage response = client.GetAsync(request).Result;
            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsAsync<BatchDTO>().Result;
            }
            else
            {
                Debug.WriteLine("Index received a bad response from the web service.");
                return null;
            }
        }

        /// <summary>
        /// Ask our Service for the batches please
        /// </summary>
        /// <param name="batches"></param>
        /// <param name="client"></param>
        /// <returns>IEnumerable<DTO.BatchDTO></returns>
        public static IEnumerable<BatchDTO> GetBatches()
        {
            HttpClient client = ApiClient();
            HttpResponseMessage response = client.GetAsync("api/Batches/All").Result;
            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsAsync<IEnumerable<BatchDTO>>().Result;
            }
            else
            {
                Debug.WriteLine("Index received a bad response from the web service.");
                return null;
            }
        }


    }
}
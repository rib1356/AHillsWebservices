using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace ImportService.ServiceLayer
{
    public class PbService
    {
        /// <summary>
        /// WE need to modify the System URi to use the deployed service
        /// </summary>
        /// <returns></returns>
        private static HttpClient ApiClient()
        {
            HttpClient client = new HttpClient();
            // http://localhost:60158/
           // client.BaseAddress = new System.Uri("http://pannebakkerupload.azurewebsites.net/");
            client.BaseAddress = new System.Uri("http://localhost:60158/");
            client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
            return client;
        }


        /// <summary>
        /// Ask our Service for the batches please
        /// </summary>
        /// <param name="batches"></param>
        /// <param name="client"></param>
        /// <returns>IEnumerable<DTO.BatchDTO></returns>
        public static IEnumerable<DTO.PbDTO> GetPbItems()
        {
            HttpClient client = ApiClient();
            HttpResponseMessage response = client.GetAsync("api/Pb/All").Result;
            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsAsync<IEnumerable<DTO.PbDTO>>().Result;
            }
            else
            {
                Debug.WriteLine("Index received a bad response from the web service.");
                return null;
            }


        }

        public static IEnumerable<DTO.PbDTO> GetPbBatchItems(int id)
        {
            HttpClient client = ApiClient();
            var request = "api/Pb/All/" + id.ToString();
            HttpResponseMessage response = client.GetAsync(request).Result;
            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsAsync<IEnumerable<DTO.PbDTO>>().Result;
            }
            else
            {
                Debug.WriteLine("Index received a bad response from the web service.");
                return null;
            }


        }


    }
}
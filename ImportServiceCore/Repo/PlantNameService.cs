using ImportServiceCore.DTO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace ImportService.ServiceLayer
{
    public class PlantNameService
    {

        /// <summary>
        /// WE need to modify the System URi to use the deployed service
        /// </summary>
        /// <returns></returns>
        private static HttpClient ApiClient()
        {
            HttpClient client = new HttpClient();
            // http://localhost:52009/
            //client.BaseAddress = new System.Uri("http://localhost:52009/");
            client.BaseAddress = new System.Uri("https://ahillsbatchservice.azurewebsites.net/");
            client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
            return client;
        }
        /// <summary>
        /// Ask our Service for the batches please
        /// </summary>
        /// <param name="batches"></param>
        /// <param name="client"></param>
        /// <returns>IEnumerable<DTO.BatchDTO></returns>
        public IEnumerable<PlantNameDTO> GetNames()
        {
            HttpClient client = ApiClient();
            HttpResponseMessage response = client.GetAsync("api/Plant").Result;
            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsAsync<IEnumerable<PlantNameDTO>>().Result;
            }
            else
            {
                Debug.WriteLine("Index received a bad response from the web service.");
                return null;
            }
        }

    }
}
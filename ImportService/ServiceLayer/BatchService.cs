using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Runtime.Caching;


namespace ImportService.ServiceLayer
{
    public class BatchService
    {
        /// <summary>
        /// WE need to modify the System URi to use the deployed service
        /// </summary>
        /// <returns></returns>
        private static HttpClient ApiClient()
        {
            HttpClient client = new HttpClient();
            // http://localhost:52009/
            // http://localhost:64383/api/Locations
            //client.BaseAddress = new System.Uri("http://localhost:52009/");
            client.BaseAddress = new System.Uri("https://ahillsbatchservice.azurewebsites.net/");
            client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
            return client;
        }

        private static HttpClient ApiLocationClient()
        {
            HttpClient client = new HttpClient();
            // http://localhost:52009/
            // http://localhost:64383/api/Locations
            //client.BaseAddress = new System.Uri("http://localhost:64383/");
            client.BaseAddress = new System.Uri("https://ahillslocationservice.azurewebsites.net/");
            client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
            return client;
        }

        // http://localhost:52009/api/Batches/All/3
        public static DTO.BatchDTO GetBatchItem(int id)
        {
            HttpClient client = ApiClient();
            var request = "api/Batches/" + id.ToString();
            HttpResponseMessage response = client.GetAsync(request).Result;
            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsAsync<DTO.BatchDTO>().Result;
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
        public static IEnumerable<DTO.BatchDTO> GetBatches()
        {
            HttpClient client = ApiClient();
            HttpResponseMessage response = client.GetAsync("api/Batches/All").Result;
            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsAsync<IEnumerable<DTO.BatchDTO>>().Result;
            }
            else
            {
                Debug.WriteLine("Index received a bad response from the web service.");
                return null;
            }
        }

        public static IEnumerable<DTO.LocationDTO> GetLocations()
        {
            
            HttpClient client = ApiLocationClient();
            var cache = new System.Runtime.Caching.MemoryCache("locations");

            var cacheItemPolicy = new CacheItemPolicy()
            {
                //Set your Cache expiration.
                AbsoluteExpiration = DateTime.Now.AddDays(1)
            };
            IEnumerable<DTO.LocationDTO> result;
            if (cache["locations"] == null)
            {
                result = GetLocationsFromervice(client); ;
                cache["locations"] = result;
            }
            else
            {
                result = (IEnumerable<DTO.LocationDTO>)cache["locations"]; 
            }

            return result;
        }

        public static IEnumerable<DTO.LocationDTO> GetSubLocations(String main)
        {

            HttpClient client = ApiLocationClient();
            
            return GetSubLocationsFromervice(client,main);
  

        }

        private static IEnumerable<DTO.LocationDTO> GetLocationsFromervice(HttpClient client)
        {
            HttpResponseMessage response = client.GetAsync("api/Locations").Result;
            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsAsync<IEnumerable<DTO.LocationDTO>>().Result;
            }
            else
            {
                Debug.WriteLine("Index received a bad response from the web service.");
                return null;
            }
        }


        private static IEnumerable<DTO.LocationDTO> GetSubLocationsFromervice(HttpClient client, String main)
        {
            //  // api/Locations?main={main}
            var query = "api/Locations?main=" + main;
            HttpResponseMessage response = client.GetAsync(query).Result;
            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsAsync<IEnumerable<DTO.LocationDTO>>().Result;
            }
            else
            {
                Debug.WriteLine("Index received a bad response from the web service.");
                return null;
            }
        }
    }
}
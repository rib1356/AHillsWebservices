using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace ImportService.ServiceLayer
{
    public class PriceItemDTO
    {
        //        <Description>=>1 and =<3</Description>
        //<MaxUnitValue>1.6</MaxUnitValue>
        //<MinUnitValue>0.4</MinUnitValue>
        //<PlantType>Pot</PlantType>
        //<RuleNumber>2</RuleNumber>
        public enum RootType
        {
            Pot,
            RootBall,
            BareRoot
        }

        public int RuleNumber { get; set; }
        public RootType PlantType { get; set; }
        public string Description { get; set; }
        public double MinUnitValue { get; set; }
        public double MaxUnitValue { get; set; }

    }

    public class PriceService
    {
        /// <summary>
        /// WE need to modify the System URi to use the deployed service
        /// </summary>
        /// <returns></returns>
        private static HttpClient ApiClient()
        {
            HttpClient client = new HttpClient();
            // http://localhost:52009/
            client.BaseAddress = new System.Uri("http://localhost:61628/");
            //client.BaseAddress = new System.Uri("https://ahillsbatchservice.azurewebsites.net/");
            client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
            return client;
        }

        public static PriceItemDTO GetPUnitPrice(string formSize)
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
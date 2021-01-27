using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using BatchModel;
using BatchService.Models;

namespace BatchService.Controllers
{
    [EnableCors("*", "*", "*")]
    public class PicklistBatchSearchController : ApiController
    {
        private HillsStockEntities db = new HillsStockEntities();

        [Route("api/search/")]
        public List<SearchResult> getPlantsBySearch(string searchQuery)
        {
            //var hillsBatches = db.Batches.Where(x => x.Location != "PB");

            var toReturn = db.Batches.Where(x => x.Name.Contains(searchQuery)).Select(x => new SearchResult
            {
                BatchId = x.Id,
                Sku = x.Sku,
                Name = x.Name,
                FormSize = x.FormSize,
                Location = x.Location,
                Quantity = x.Quantity,
                GrowingQuantity = x.GrowingQuantity ?? 0,
                AllocatedQuantity = x.AllocatedQuantity ?? 0,
                WholesalePrice = x.WholesalePrice,
                Active = x.Active,
            }).ToList();

            return toReturn;
        }

        public class SearchResult
        {
            public int BatchId { get; set; }
            public string Sku { get; set; }
            public string Name { get; set; }
            public string FormSize { get; set; }
            public string Location { get; set; }
            public int Quantity { get; set; }
            public int GrowingQuantity { get; set; }
            public int AllocatedQuantity { get; set; }
            public int? WholesalePrice { get; set; }
            public bool Active { get; set; }

        }
    }
}
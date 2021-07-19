using PriceRules;
using FormPriceRules;
using quoteService.DTO;
using quoteService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace quoteService.Controllers
{
    
    [EnableCors("*", "*", "*")]
 
    public class FormPricingController : ApiController
    {
        private HillsStock1Entities db = new HillsStock1Entities();

        public IEnumerable<FormSizePriceItem> GetPrices(IEnumerable<BatchQuoteItem> items)
        {
            //var 
            IQueryable<Batch> results = GetBatches(items);

            List<FormSizePriceItem> outList = new List<FormSizePriceItem>();

            /// build output list
            foreach (var item in results)
            {
                // thisDTO object will hold the result of the form based price calculation
                FormPriceRules.PriceItemDTO thisDTO = new PriceItemDTO();

                // result is the Item we are going to sendback
                var result = new FormSizePriceItem();

                // my is is..
                result.BatchId = item.Id;
                // my starting price is..
                result.PriceIn = items.First(i => i.BatchId == item.Id).PriceIn;

                // calculate the formsize price max and min the client uses int 1234 the calculator uses doubles
                // 12.34
                thisDTO = FormPriceRules.PriceService.GetUnitPrice(item.FormSize, item.FormSizeCode);
                int max = Convert.ToInt32(thisDTO.MaxUnitValue * 100);
                int min = Convert.ToInt32(thisDTO.MinUnitValue * 100);

                // test and cap
                result.PriceOut = result.PriceIn;
                if (max <= result.PriceIn)
                {
                    result.PriceOut = max;
                }
                if (min >= result.PriceIn)
                {
                    result.PriceOut = min;
                }
                result.Description = thisDTO.RuleNumber + " was applied to " + thisDTO.PlantType + " with a max of " + thisDTO.MaxUnitValue + " a min of " + thisDTO.MinUnitValue + "Description " + thisDTO.Description;

                outList.Add(result);

            }

            return outList;

        }

        private IQueryable<Batch> GetBatches(IEnumerable<BatchQuoteItem> items)
        {
            List<int> batchIds = new List<int>();
            foreach (var i in items)
            {
                batchIds.Add(i.BatchId);
            }
            var results = db.Batches.Where(u => batchIds.Contains(u.Id));
            return results;
        }
    }
}

using BatchModel;
using FormSizeService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;
using System.Web.Http.Cors;

namespace BatchService.Controllers
{
    [EnableCors("*", "*", "*")]
    public class BatchFilterController : ApiController
    {
        private HillsStockEntities db = new HillsStockEntities();

 
        public List<FormSizeService.TypeModel> GetTypes()
        {
            List<FormSizeService.TypeModel> list;
            list = FormSizeService.FormType.buildList();
            return list;
        }

        public List<Batch> GetBatchesByForm(FormType.RootType type)
        {
            
            var testBatches = db.Batches.ToList();
            IEnumerable<Batch> filter;
            switch (type)
            {
                case FormType.RootType.Bulb:
                    filter = from b in testBatches
                                 where FormSizeService.FormFilter.isBulb(b)
                                 select b;
                    return filter.ToList();
                case FormType.RootType.RootBall:
                    filter = from b in testBatches
                             where FormSizeService.FormFilter.IsRootBall(b)
                             select b;
                    return filter.ToList();
                case FormType.RootType.Topiary:
                    filter = from b in testBatches
                             where FormSizeService.FormFilter.IsTopiary(b)
                             select b;
                    return filter.ToList();
                case FormType.RootType.Pot:
                    filter = from b in testBatches
                             where FormSizeService.FormFilter.isPot(b)
                             select b;
                    return filter.ToList();
                case FormType.RootType.BareRoot:
                    filter = from b in testBatches
                             where FormSizeService.FormFilter.isBareRoot(b)
                             select b;
                    return filter.ToList();
                default:
                    return testBatches.ToList();
            }
            return null;


        }
    }

  
}

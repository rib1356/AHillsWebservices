using ImportRep;
using ImportService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace ImportService.Controllers
{
    [EnableCors("*", "*", "*")]
    public class PbServiceController : ApiController
    {
       // private HillsStockImportEntities db = new HillsStockImportEntities();

        private IImportRepository db;

        public PbServiceController()
        {
            this.db = new ImportRepository(new ImportModel.ImportEntities());
        }



        [Route("api/Pb/All")]
        // GET: api/Batches
        /// Send a collection of active BatchItemDTO's 
        /// Currently purchase Price does not exist in domain
        public IEnumerable<DTO.PbDTO> GetAllPbItems()
        {
            var all = db.GetPannebakkers();
            var result = all.Select(item => new DTO.PbDTO
            {
                Sku = item.Sku,
                Name = item.Name,
                FormSize = item.FormSize,
                FormSizeCode = item.FormSizeCode,
                Price = item.Price

            });
            var fred = result.ToList();
            return result;
        }



        [Route("api/Pb/All/{id}")]
        public IEnumerable<DTO.PbDTO> GetBatchPbItems(int id)
        {
            var all = db.GetPannebakkers().Where(p => p.BatchId == id);
            var result = all.Select(item => new DTO.PbDTO
            {
                Sku = item.Sku,
                Name = item.Name,
                FormSize = item.FormSize,
                FormSizeCode = item.FormSizeCode,
                Price = item.Price

            });
            var fred = result.ToList();
            return result;
        }

    }
}

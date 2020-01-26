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
    [RoutePrefix("api/Pb")]
    public class PbServiceController : ApiController
    {
       // private HillsStockImportEntities db = new HillsStockImportEntities();

        private IImportRepository db;

        public PbServiceController()
        {
            this.db = new ImportRepository(new ImportModel.ImportEntities());
        }


        [Route("{firstLetter}")]
        public IEnumerable<DTO.PbDTO> GetStartsWith(string firstLetter)
        {

            var all = db.GetPannebakkers().Where(x => x.Name.StartsWith(firstLetter, StringComparison.OrdinalIgnoreCase)); ;
            IEnumerable<DTO.PbDTO> result = BuildDTOs(all);
            return result;

        }



        [Route("filter/{name?}/{sku?}")]
        public IEnumerable<DTO.PbDTO> Filters(string name, string sku)
        {
            IEnumerable<ImportModel.Pannebakker> result = null;
            var hasName = !(String.IsNullOrEmpty(name));
            var hasSku = !(String.IsNullOrEmpty(sku));
            var ALL = db.GetPannebakkers();
            if ( hasName && hasSku)
                {
                    result = ALL.Where(i => i.Name == name && i.Sku == sku);
                }
            if (hasName && !(hasSku))
                {
                    result = ALL.Where(i => i.Name == name);
                }
            if (!(hasName) && hasSku)
                {
                    result = ALL.Where(i => i.Sku == sku);
                }

            IEnumerable<DTO.PbDTO> dto = BuildDTOs(result);

            return dto;
        }

        private static IEnumerable<DTO.PbDTO> BuildDTOs(IEnumerable<ImportModel.Pannebakker> all)
        {
            return all.Select(item => new DTO.PbDTO
            {
                Sku = item.Sku,
                Name = item.Name,
                FormSize = item.FormSize,
                FormSizeCode = item.FormSizeCode,
                Price = item.Price

            });
        }

        [Route("All")]
        // GET: api/Batches
        /// Send a collection of active BatchItemDTO's 
        /// Currently purchase Price does not exist in domain
        public IEnumerable<DTO.PbDTO> GetAllPbItems()
        {
            var all = db.GetPannebakkers();
            IEnumerable<DTO.PbDTO> result = BuildDTOs(all);
            return result;
        }



        [Route("All/{id}")]
        public IEnumerable<DTO.PbDTO> GetBatchPbItems(int id)
        {
            var all = db.GetPannebakkers().Where(p => p.BatchId == id);
            IEnumerable<DTO.PbDTO> result = BuildDTOs(all);
            var fred = result.ToList();
            return result;
        }

    }
}

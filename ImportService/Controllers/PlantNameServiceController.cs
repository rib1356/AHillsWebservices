using ImportModel;
using ImportService.DTO;
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
    public class PlantNameServiceController : ApiController
    {
        private ImportEntities db = new ImportEntities();

        public IQueryable<PlantNameDTO> GetAllBatches()
        {
            var all = db.PlantNames.ToList();
            List<PlantNameDTO> DTO = new List<PlantNameDTO>();
            foreach (var item in all)
            {
                var b = new PlantNameDTO
                {
                    plantName = item.Name,
                     Sku = item.Sku

                };
                DTO.Add(b);


            };
            return DTO.AsQueryable();
        }


    }
}

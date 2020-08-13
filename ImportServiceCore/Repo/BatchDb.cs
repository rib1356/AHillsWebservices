using ImportServiceCore.DTO;
using ImportServiceCore.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImportServiceCore.Repo
{
    public class BatchDb : IBatchRep
    {
        private HillsContext _context;

        public BatchDb(HillsContext Context)
        {
            this._context = Context;
        }

        public IEnumerable<BatchDTO> GetBatches()
        {
            var raw = _context.Batch;
            return buildVM(raw);

        }

        private static List<DTO.BatchDTO> buildVM(Microsoft.EntityFrameworkCore.DbSet<Batch> batches)
        {
            return batches.Select(b => new DTO.BatchDTO
            {       
                 Id = b.Id,
                Sku = b.Sku,
                Name = b.Name,
                FormSize = b.FormSize,
                Quantity = b.Quantity,
                Location = b.Location,
                WholesalePrice = b.WholesalePrice
            }).ToList();
        }

        public BatchDTO GetBatchItem(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LocationDTO> GetLocations()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LocationDTO> GetSubLocations(string main)
        {
            throw new NotImplementedException();
        }
    }
}

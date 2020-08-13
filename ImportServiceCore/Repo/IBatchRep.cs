using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImportServiceCore.Repo
{

        public interface IBatchRep
        {
            
            IEnumerable<DTO.BatchDTO> GetBatches();
            DTO.BatchDTO GetBatchItem(int id);
            IEnumerable<DTO.LocationDTO> GetLocations();
            IEnumerable<DTO.LocationDTO> GetSubLocations(String main);


        }

}

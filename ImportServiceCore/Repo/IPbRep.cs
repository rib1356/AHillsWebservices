using ImportServiceCore.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImportServiceCore.Repo
{
    public interface IPbRep
    {
    
        IEnumerable<DTO.PbDTO> GetPbItems();
        IEnumerable<DTO.PbDTO> GetPbBatchItems(int id);
        IEnumerable<Pannebakker> GetPbRawItems();

    }
}

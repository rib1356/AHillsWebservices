using ImportServiceCore.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImportServiceCore.Repo
{
    public interface IPriceRepo
    {
        PriceItemDTO GetPUnitPrice(string formSize);
    }
}

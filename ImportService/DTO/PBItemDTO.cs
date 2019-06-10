using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImportService.DTO
{
    public class PBItemDTO
    {
        public string Sku { get; set; }
        public string FormSizeCode { get; set; }
        public string Name { get; set; }
        public string FormSize { get; set; }
        public decimal Price { get; set; }
        public Nullable<bool> RootBall { get; set; }
    }
}
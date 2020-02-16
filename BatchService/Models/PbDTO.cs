using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BatchService.Models
{

    public class PannebakkerDTO
    {
        public int Id { get; set; } 
        public string Sku { get; set; }
        public string Name { get; set; }
        public string FormSize { get; set; }
        public string FormSizeCode { get; set; }
        public int Price { get; set; } //This is the purchasing price for Ahills
        public string Location { get; set; }
        public bool RootBall { get; set; }
        public int BatchId { get; set; }

    }
}
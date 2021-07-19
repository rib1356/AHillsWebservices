using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace quoteService.DTO
{
    public class GrossProfitMarginDTO
    {
        public int gpmId { get; set; }
        public decimal rowMin { get; set; }
        public decimal rowMax { get; set; }
        public int ruleSet { get; set; }
    }
}
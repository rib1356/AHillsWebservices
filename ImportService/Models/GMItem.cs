using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImportService.Models
{
    public class GMItem
    {
        // 1
        public string PBFSCode { get; set; }
        // 2
        public string Sku { get; set; }
        // 3
        public string Name { get; set; }
        // 4
        public string Size { get; set; }
        // 5
        public string Form { get; set; }
        // 6
        public string Location { get; set; }
        // 7
        public int StockQuantity { get; set; }
        // 8
        public int AllocatedQuantity { get; set; }
        // 9
        public int GrowingQuantity { get; set; }
        // 12
        public decimal WholeSalePrice { get; set; }
    }
}
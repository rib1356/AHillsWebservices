using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ImportServiceCore.DTO
{
    public class BatchDTO
    {
        public int Id { get; set; }
        public string Sku { get; set; }
        public string Name { get; set; }
        public string FormSize { get; set; }
        public string Location { get; set; }
        public int Quantity { get; set; }
        public int GrowingQuantity { get; set; }
        public Nullable<int> WholesalePrice { get; set; }

    }

  


}
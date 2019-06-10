using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ImportService.DTO
{
    public class BatchDTO
    {
        public string Sku { get; set; }
        public string Name { get; set; }
        public string FormSize { get; set; }
        public int Quantity { get; set; }
        public Nullable<int> WholesalePrice { get; set; }

    }


    public class BatchVM
    {
        [Display(Name="SKU")]
        public string Sku { get; set; }
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Display(Name = "Form Size")]
        public string FormSize { get; set; }
        public int Quantity { get; set; }
        [Display(Name = "Price")]
        public Nullable<int> WholesalePrice { get; set; }

    }
}
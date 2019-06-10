using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ImportService.DTO
{
    public class PbDTO
    {
        public string Sku { get; set; }
        public string FormSizeCode { get; set; }
        public string Name { get; set; }
        public string FormSize { get; set; }
        public decimal Price { get; set; }

    }

    public class PbVM
    {
        [Display(Name = "SKU")]
        public string Sku { get; set; }
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Display(Name = "Form Size")]
        public string FormSizeCode { get; set; }
        [Display(Name = "Price")]
        public decimal Price { get; set; }

    }
}
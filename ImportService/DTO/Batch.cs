using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ImportService.DTO
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

    public class BatchPBVM
    {
        public BatchVM BatchItem { get; set; }
        public IEnumerable<PbVM> PbList {get;set;}
    }

    public class BatchVM
    {
        public int BatchId { get; set; }
        [Display(Name="SKU")]
        public string Sku { get; set; }
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Display(Name = "Location")]
        public string Location { get; set; }
        [Display(Name = "Form Size")]
        public string FormSize { get; set; }
        [Display(Name = "Quantity")]
        public int Quantity { get; set; }
        [Display(Name = "Price")]
        public Nullable<decimal> WholesalePrice { get; set; }

    }

    public class BatchEditVM
    {
        public int BatchId { get; set; }
        [Display(Name = "SKU")]
        public string Sku { get; set; }
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Display(Name = "Location")]
        public string MainLocation { get; set; }
        [Display(Name = "Sub Location")]
        public string SubLocation { get; set; }
        [Display(Name = "Form Size")]
        public string FormSize { get; set; }
        [Display(Name = "Quantity")]
        public int Quantity { get; set; }
        public bool wasPB { get; set; }
        [Display(Name = "Select if for Sale")]
        public bool forSale { get; set; }
       

    }


}
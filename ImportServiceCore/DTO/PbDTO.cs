using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ImportServiceCore.DTO
{
    public class PbDTO
    {
        public int PbId { get; set; }
        public int BatchId { get; set; }
        public string Sku { get; set; }
        public string FormSizeCode { get; set; }
        public string Name { get; set; }
        public string FormSize { get; set; }
        public decimal Price { get; set; }

    }

   

    public class PbVM
    {
        public int PbId { get; set; }
        public int BatchId { get; set; }
        [Display(Name = "SKU")]
        public string Sku { get; set; }
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Display(Name = "Form Size")]
        public string FormSizeCode { get; set; }
        [Display(Name = "Price")]
        public decimal Price { get; set; }

        public static  IEnumerable<PbVM> buildVM(IEnumerable<PbDTO> pBbatches)
        {
            return pBbatches.Select(b => new DTO.PbVM
            {
                PbId = b.PbId,
                BatchId = b.BatchId,
                Sku = b.Sku,
                Name = b.Name,
                FormSizeCode = b.FormSizeCode,
                Price = b.Price
            }).AsEnumerable();
        }

    }
}
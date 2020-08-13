using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HillsCoreModel
{
    public partial class Batch
    {
        public int Id { get; set; }
        [Required]
        [StringLength(255)]
        public string Sku { get; set; }
        [Required]
        [StringLength(255)]
        public string Name { get; set; }
        [StringLength(255)]
        public string FormSize { get; set; }
        [Required]
        [StringLength(255)]
        public string Location { get; set; }
        public int Quantity { get; set; }
        public int? WholesalePrice { get; set; }
        public bool? ImageExists { get; set; }
        public bool Active { get; set; }
        public int? GrowingQuantity { get; set; }
        public int? AllocatedQuantity { get; set; }
        [Column(TypeName = "date")]
        public DateTime? DateStamp { get; set; }
        [Column(TypeName = "decimal(18, 0)")]
        public decimal? BuyPrice { get; set; }
        public string Comment { get; set; }
    }
}

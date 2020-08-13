using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HillsCoreModel
{
    public partial class Pannebakker
    {
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Sku { get; set; }
        [Required]
        [StringLength(50)]
        public string FormSizeCode { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [StringLength(50)]
        public string FormSize { get; set; }
        [Column(TypeName = "decimal(18, 0)")]
        public decimal Price { get; set; }
        public bool? RootBall { get; set; }
        public int? BatchId { get; set; }
        public string Location { get; set; }
    }
}

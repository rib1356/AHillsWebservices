using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImportServiceCore.Model
{
    public partial class Quotes
    {
        public Quotes()
        {
            PlantsForQuote = new HashSet<PlantsForQuote>();
        }

        [Key]
        public int QuoteId { get; set; }
        [Required]
        [StringLength(50)]
        public string CustomerReference { get; set; }
        [Column(TypeName = "date")]
        public DateTime? QuoteDate { get; set; }
        public string SiteReference { get; set; }
        public int? QuotePrice { get; set; }
        [Column(TypeName = "date")]
        public DateTime? QuoteExpiryDate { get; set; }
        public bool Active { get; set; }
        public bool SalesOrder { get; set; }
        public bool Retail { get; set; }

        [InverseProperty("Quote")]
        public ICollection<PlantsForQuote> PlantsForQuote { get; set; }
    }
}

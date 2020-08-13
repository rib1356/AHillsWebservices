using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HillsCoreModel
{
    public partial class PlantsForQuote
    {
        public int PlantsForQuoteId { get; set; }
        public string PlantName { get; set; }
        public string FormSize { get; set; }
        public string Comment { get; set; }
        public int? Price { get; set; }
        public int? Quantity { get; set; }
        public int QuoteId { get; set; }
        public bool Active { get; set; }

        [ForeignKey("QuoteId")]
        [InverseProperty("PlantsForQuote")]
        public Quotes Quote { get; set; }
    }
}

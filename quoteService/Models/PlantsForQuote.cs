//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace quoteService.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class PlantsForQuote
    {
        public int PlantsForQuoteId { get; set; }
        public string PlantName { get; set; }
        public string FormSize { get; set; }
        public string Comment { get; set; }
        public Nullable<int> Price { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<int> QuoteId { get; set; }
    
        public virtual Quote Quote { get; set; }
    }
}

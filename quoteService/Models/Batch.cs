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
    
    public partial class Batch
    {
        public int Id { get; set; }
        public string Sku { get; set; }
        public string Name { get; set; }
        public string FormSize { get; set; }
        public string Location { get; set; }
        public int Quantity { get; set; }
        public Nullable<int> WholesalePrice { get; set; }
        public Nullable<bool> ImageExists { get; set; }
        public bool Active { get; set; }
        public Nullable<int> GrowingQuantity { get; set; }
        public Nullable<int> AllocatedQuantity { get; set; }
        public Nullable<System.DateTime> DateStamp { get; set; }
        public Nullable<decimal> BuyPrice { get; set; }
        public string Comment { get; set; }
        public string FormSizeCode { get; set; }
    }
}

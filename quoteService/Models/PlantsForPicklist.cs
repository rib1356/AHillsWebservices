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
    
    public partial class PlantsForPicklist
    {
        public int PlantForPickListId { get; set; }
        public int PicklistId { get; set; }
        public int PlantForQuoteId { get; set; }
        public string PlantName { get; set; }
        public string FormSize { get; set; }
        public int QuantityToPick { get; set; }
        public string SubbedFor { get; set; }
        public string DispatchLocation { get; set; }
        public bool Active { get; set; }
        public bool isSubbed { get; set; }
        public int BatchId { get; set; }
    
        public virtual Picklist Picklist { get; set; }
    }
}
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
    
    public partial class Picklist
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Picklist()
        {
            this.PlantsForPicklists = new HashSet<PlantsForPicklist>();
        }
    
        public int PicklistId { get; set; }
        public int QuoteId { get; set; }
        public System.DateTime DispatchDate { get; set; }
        public string DeliveryAddress { get; set; }
        public bool DeliveryNeeded { get; set; }
        public bool IsPicked { get; set; }
        public bool Active { get; set; }
        public bool IsPacked { get; set; }
        public bool IsDelivered { get; set; }
        public string Comment { get; set; }
        public bool EstimatedDelivery { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PlantsForPicklist> PlantsForPicklists { get; set; }
    }
}

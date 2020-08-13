using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImportServiceCore.Model
{
    public partial class Picklist
    {
        public Picklist()
        {
            PlantsForPicklist = new HashSet<PlantsForPicklist>();
        }

        public int PicklistId { get; set; }
        public int QuoteId { get; set; }
        [Column(TypeName = "date")]
        public DateTime DispatchDate { get; set; }
        public string DeliveryAddress { get; set; }
        public bool DeliveryNeeded { get; set; }
        public bool IsAllocated { get; set; }
        public bool Active { get; set; }
        public bool IsPicked { get; set; }
        public bool IsDelivered { get; set; }
        public string Comment { get; set; }
        public bool EstimatedDelivery { get; set; }

        [InverseProperty("Picklist")]
        public ICollection<PlantsForPicklist> PlantsForPicklist { get; set; }
    }
}

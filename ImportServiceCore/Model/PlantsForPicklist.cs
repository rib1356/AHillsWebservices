using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImportServiceCore.Model
{
    public partial class PlantsForPicklist
    {
        [Key]
        public int PlantForPickListId { get; set; }
        public int PicklistId { get; set; }
        public int PlantForQuoteId { get; set; }
        [Required]
        public string PlantName { get; set; }
        public string FormSize { get; set; }
        public int QuantityToPick { get; set; }
        public string OriginalItem { get; set; }
        public string DispatchLocation { get; set; }
        public bool Active { get; set; }
        public bool IsSubbed { get; set; }
        public int BatchId { get; set; }
        public int QuantityPicked { get; set; }

        [ForeignKey("PicklistId")]
        [InverseProperty("PlantsForPicklist")]
        public Picklist Picklist { get; set; }
    }
}

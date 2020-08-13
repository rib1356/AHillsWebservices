using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImportServiceCore.Model
{
    public partial class PlantNames
    {
        public PlantNames()
        {
            PlantGroup = new HashSet<PlantGroup>();
        }

        [Key]
        public int PlantId { get; set; }
        [Required]
        [StringLength(255)]
        public string Sku { get; set; }
        [StringLength(255)]
        public string Name { get; set; }

        [InverseProperty("Plant")]
        public ICollection<PlantGroup> PlantGroup { get; set; }
    }
}

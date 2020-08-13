using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImportServiceCore.Model
{
    public partial class PlantGroup
    {
        public int PlantGroupId { get; set; }
        public int PlantId { get; set; }
        public int GroupId { get; set; }

        [ForeignKey("GroupId")]
        [InverseProperty("PlantGroup")]
        public Groups Group { get; set; }
        [ForeignKey("PlantId")]
        [InverseProperty("PlantGroup")]
        public PlantNames Plant { get; set; }
    }
}

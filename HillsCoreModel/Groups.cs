using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HillsCoreModel
{
    public partial class Groups
    {
        public Groups()
        {
            FormSizes = new HashSet<FormSizes>();
            PlantGroup = new HashSet<PlantGroup>();
        }

        [Key]
        public int GroupId { get; set; }
        [StringLength(255)]
        public string Description { get; set; }

        [InverseProperty("Group")]
        public ICollection<FormSizes> FormSizes { get; set; }
        [InverseProperty("Group")]
        public ICollection<PlantGroup> PlantGroup { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HillsCoreModel
{
    public partial class FormSizes
    {
        [Key]
        public int FormSizeId { get; set; }
        public int GroupId { get; set; }
        [StringLength(255)]
        public string Age { get; set; }
        [StringLength(255)]
        public string HeightSpread { get; set; }
        [StringLength(255)]
        public string Girth { get; set; }
        public double? PotSize { get; set; }
        [StringLength(255)]
        public string RootType { get; set; }
        [StringLength(255)]
        public string Description { get; set; }
        public bool Active { get; set; }

        [ForeignKey("GroupId")]
        [InverseProperty("FormSizes")]
        public Groups Group { get; set; }
    }
}

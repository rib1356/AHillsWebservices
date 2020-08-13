using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImportServiceCore.Model
{
    public partial class Location
    {
        public int LocationId { get; set; }
        [Required]
        [StringLength(255)]
        public string MainLocation { get; set; }
        [Required]
        [StringLength(255)]
        public string SubLocation { get; set; }
        public int? WalkOrder { get; set; }
        public bool? Active { get; set; }
    }
}

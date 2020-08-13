using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImportServiceCore.Model
{
    public partial class CustomerInformation
    {
        [Key]
        public int CustomerId { get; set; }
        [Required]
        [StringLength(255)]
        public string CustomerName { get; set; }
        [StringLength(20)]
        public string CustomerTel { get; set; }
        public string CustomerAddress { get; set; }
        [StringLength(75)]
        public string CustomerEmail { get; set; }
        [Required]
        [StringLength(50)]
        public string CustomerReference { get; set; }
        public bool? SageCustomer { get; set; }
        [StringLength(20)]
        public string CustomerTel2 { get; set; }
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PlantService.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class FormSize
    {
        public int FormSizeId { get; set; }
        public int GroupId { get; set; }
        public string Age { get; set; }
        public string HeightSpread { get; set; }
        public string Girth { get; set; }
        public Nullable<double> PotSize { get; set; }
        public string RootType { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
    
        public virtual Group Group { get; set; }
    }
}

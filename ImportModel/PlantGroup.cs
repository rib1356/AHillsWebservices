//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ImportModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class PlantGroup
    {
        public int PlantGroupId { get; set; }
        public int PlantId { get; set; }
        public int GroupId { get; set; }
    
        public virtual Group Group { get; set; }
        public virtual PlantName PlantName { get; set; }
    }
}
﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class HillsStock1Entities : DbContext
    {
        public HillsStock1Entities()
            : base("name=HillsStock1Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<FormSize> FormSizes { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<PlantGroup> PlantGroups { get; set; }
        public virtual DbSet<PlantName> PlantNames { get; set; }
    }
}

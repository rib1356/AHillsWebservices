﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace quoteService.Models
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
    
        public virtual DbSet<CustomerInformation> CustomerInformations { get; set; }
        public virtual DbSet<PlantsForQuote> PlantsForQuotes { get; set; }
        public virtual DbSet<Quote> Quotes { get; set; }
        public virtual DbSet<Picklist> Picklists { get; set; }
        public virtual DbSet<PlantsForPicklist> PlantsForPicklists { get; set; }
        public virtual DbSet<Batch> Batches { get; set; }
    }
}

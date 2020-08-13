using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ImportServiceCore.Model
{
    public partial class HillsContext : DbContext
    {
        public virtual DbSet<Batch> Batch { get; set; }
        public virtual DbSet<CustomerInformation> CustomerInformation { get; set; }
        public virtual DbSet<FormSizes> FormSizes { get; set; }
        public virtual DbSet<Groups> Groups { get; set; }
        public virtual DbSet<Location> Location { get; set; }
        public virtual DbSet<Pannebakker> Pannebakker { get; set; }
        public virtual DbSet<Picklist> Picklist { get; set; }
        public virtual DbSet<PlantGroup> PlantGroup { get; set; }
        public virtual DbSet<PlantNames> PlantNames { get; set; }
        public virtual DbSet<PlantsForPicklist> PlantsForPicklist { get; set; }
        public virtual DbSet<PlantsForQuote> PlantsForQuote { get; set; }
        public virtual DbSet<Quotes> Quotes { get; set; }
        public virtual DbSet<RawImport> RawImport { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
               //To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer(@"Server=tcp:hills-server.database.windows.net,1433;Database=HillsStock1;User ID=rib1356;Password=A-Hills-Stock;Encrypt=true;Connection Timeout=30;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Batch>(entity =>
            {
                entity.Property(e => e.BuyPrice).HasDefaultValueSql("((0))");

                entity.Property(e => e.DateStamp).HasDefaultValueSql("('2019-02-27')");
            });

            modelBuilder.Entity<CustomerInformation>(entity =>
            {
                entity.Property(e => e.SageCustomer).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<FormSizes>(entity =>
            {
                entity.HasOne(d => d.Group)
                    .WithMany(p => p.FormSizes)
                    .HasForeignKey(d => d.GroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FormSizes_Groups");
            });

            modelBuilder.Entity<PlantGroup>(entity =>
            {
                entity.HasOne(d => d.Group)
                    .WithMany(p => p.PlantGroup)
                    .HasForeignKey(d => d.GroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PlantGroup_Groups");

                entity.HasOne(d => d.Plant)
                    .WithMany(p => p.PlantGroup)
                    .HasForeignKey(d => d.PlantId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PlantGroup_PlantNames");
            });

            modelBuilder.Entity<PlantsForPicklist>(entity =>
            {
                entity.Property(e => e.QuantityPicked).HasDefaultValueSql("((0))");

                entity.HasOne(d => d.Picklist)
                    .WithMany(p => p.PlantsForPicklist)
                    .HasForeignKey(d => d.PicklistId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PlantsForPicklist_Picklist");
            });

            modelBuilder.Entity<PlantsForQuote>(entity =>
            {
                entity.Property(e => e.Comment).IsUnicode(false);

                entity.Property(e => e.FormSize).IsUnicode(false);

                entity.Property(e => e.PlantName).IsUnicode(false);

                entity.HasOne(d => d.Quote)
                    .WithMany(p => p.PlantsForQuote)
                    .HasForeignKey(d => d.QuoteId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PlantsForQuote_Quotes");
            });
        }
    }
}

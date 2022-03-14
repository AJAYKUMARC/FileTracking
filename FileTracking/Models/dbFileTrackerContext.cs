using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace FileTracking.Models
{
    public partial class dbFileTrackerContext : DbContext
    {
        public dbFileTrackerContext()
        {
        }

        public dbFileTrackerContext(DbContextOptions<dbFileTrackerContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Master> Masters { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=AJAYKUMARM2;Database=dbFileTracker;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Master>(entity =>
            {
                entity.ToTable("master");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Barcode)
                    .HasMaxLength(350)
                    .IsUnicode(false)
                    .HasColumnName("BARCODE");

                entity.Property(e => e.Comment)
                    .HasMaxLength(350)
                    .IsUnicode(false)
                    .HasColumnName("COMMENT");

                entity.Property(e => e.Department)
                    .HasMaxLength(350)
                    .IsUnicode(false)
                    .HasColumnName("DEPARTMENT");

                entity.Property(e => e.Filename)
                    .HasMaxLength(350)
                    .IsUnicode(false)
                    .HasColumnName("FILENAME");

                entity.Property(e => e.Uploaddate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPLOADDATE");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

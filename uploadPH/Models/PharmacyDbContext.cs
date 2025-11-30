using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Pharmacy.Models;

public partial class PharmacyDbContext : DbContext
{
    public PharmacyDbContext()
    {
    }

    public PharmacyDbContext(DbContextOptions<PharmacyDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<AdminLog> AdminLogs { get; set; }

    public virtual DbSet<Drug> Drugs { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<InvoiceItem> InvoiceItems { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-GF59DLG;Database=PharmacyDB;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.AdminId).HasName("PK__admins__43AA41416CCF523B");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
        });

        modelBuilder.Entity<AdminLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK__admin_lo__9E2397E0F7D5B120");

            entity.Property(e => e.ActionTime).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Admin).WithMany(p => p.AdminLogs).HasConstraintName("fk_admin_logs_admin");
        });

        modelBuilder.Entity<Drug>(entity =>
        {
            entity.HasKey(e => e.DrugId).HasName("PK__drugs__73F2330C5D9AD3E4");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.IsLow).HasComputedColumnSql("(case when (([shelf_amount]+[stored_amount])*[amount_per_sub]+[amount_per_sub_left])<[low_threshold] then CONVERT([bit],(1)) else CONVERT([bit],(0)) end)", false);
            entity.Property(e => e.LowThreshold).HasDefaultValue(10);
            entity.Property(e => e.TotalInSub).HasComputedColumnSql("(([shelf_amount]+[stored_amount])*[amount_per_sub]+[amount_per_sub_left])", true);

            entity.HasMany(d => d.Tags).WithMany(p => p.Drugs)
                .UsingEntity<Dictionary<string, object>>(
                    "DrugTag",
                    r => r.HasOne<Tag>().WithMany()
                        .HasForeignKey("TagId")
                        .HasConstraintName("fk_dt_tag"),
                    l => l.HasOne<Drug>().WithMany()
                        .HasForeignKey("DrugId")
                        .HasConstraintName("fk_dt_drug"),
                    j =>
                    {
                        j.HasKey("DrugId", "TagId").HasName("PK__drug_tag__07DB59278AB2A004");
                        j.ToTable("drug_tags");
                        j.IndexerProperty<int>("DrugId").HasColumnName("drug_id");
                        j.IndexerProperty<int>("TagId").HasColumnName("tag_id");
                    });
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.InvoiceId).HasName("PK__invoices__F58DFD496D5DDABB");

            entity.Property(e => e.InvoiceTime).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Admin).WithMany(p => p.Invoices)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_invoice_admin");
        });

        modelBuilder.Entity<InvoiceItem>(entity =>
        {
            entity.HasKey(e => e.ItemId).HasName("PK__invoice___52020FDDCCC3C26A");

            entity.Property(e => e.TotalPrice).HasComputedColumnSql("([quantity]*[unit_price])", true);

            entity.HasOne(d => d.Drug).WithMany(p => p.InvoiceItems).HasConstraintName("fk_ii_drug");

            entity.HasOne(d => d.Invoice).WithMany(p => p.InvoiceItems).HasConstraintName("fk_ii_invoice");
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.TagId).HasName("PK__tags__4296A2B69CF2E0C4");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

using Microsoft.EntityFrameworkCore;
using Pharmacy.Models;

namespace Pharmacy.Data
{
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

        // --- NEW: DbSet for the explicit join table ---
        public virtual DbSet<DrugTag> DrugTags { get; set; }
        // ----------------------------------------------

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Admin>(entity =>
            {
                entity.HasKey(e => e.AdminId).HasName("PK__admins__43AA4141A585EE2F");
                entity.ToTable("admins");
                entity.HasIndex(e => e.Email, "UQ__admins__AB6E6164FF3CA0A0").IsUnique();
                entity.HasIndex(e => e.Username, "UQ__admins__F3DBC57214218078").IsUnique();
                entity.Property(e => e.AdminId).HasColumnName("admin_id");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())").HasColumnName("created_at");
                entity.Property(e => e.Email).HasMaxLength(255).HasColumnName("email");
                entity.Property(e => e.PasswordHash).HasMaxLength(512).HasColumnName("password_hash");
                entity.Property(e => e.Role).HasMaxLength(50).HasColumnName("role");
                entity.Property(e => e.Username).HasMaxLength(255).HasColumnName("username");
            });


            modelBuilder.Entity<AdminLog>(entity =>
            {
                entity.HasKey(e => e.LogId).HasName("PK__admin_lo__9E2397E0E51EA211");
                entity.ToTable("admin_logs");
                entity.Property(e => e.LogId).HasColumnName("log_id");
                entity.Property(e => e.ActionTime).HasDefaultValueSql("(sysutcdatetime())").HasColumnName("action_time");
                entity.Property(e => e.ActionType).HasMaxLength(50).HasColumnName("action_type");
                entity.Property(e => e.AdminId).HasColumnName("admin_id");
                entity.HasOne(d => d.Admin).WithMany(p => p.AdminLogs)
                    .HasForeignKey(d => d.AdminId)
                    .HasConstraintName("fk_admin_logs_admin");
            });

            modelBuilder.Entity<Drug>(entity =>
            {
                entity.HasKey(e => e.DrugId).HasName("PK__drugs__73F2330CA7F8E910");
                entity.ToTable("drugs");
                entity.HasIndex(e => e.Barcode, "UQ__drugs__C16E36F80ED4F771").IsUnique();
                entity.Property(e => e.DrugId).HasColumnName("drug_id");
                entity.Property(e => e.Barcode).HasMaxLength(255).HasColumnName("barcode");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())").HasColumnName("created_at");
                entity.Property(e => e.DescriptionBeforeUse).HasColumnName("description_before_use");
                entity.Property(e => e.DescriptionHowToUse).HasColumnName("description_how_to_use");
                entity.Property(e => e.DescriptionSideEffects).HasColumnName("description_side_effects");
                entity.Property(e => e.DrugType).HasMaxLength(50).HasColumnName("drug_type");
                entity.Property(e => e.ExpirationDate).HasColumnName("expiration_date");
                entity.Property(e => e.ImageUrl).HasMaxLength(1000).HasColumnName("image_url");
                entity.Property(e => e.LowAmount).HasDefaultValue(10).HasColumnName("low_amount");
                entity.Property(e => e.Manufacturer).HasMaxLength(255).HasColumnName("manufacturer");
                entity.Property(e => e.Name).HasMaxLength(255).HasColumnName("name");
                entity.Property(e => e.PurchasingPrice).HasColumnType("decimal(18, 2)").HasColumnName("purchasing_price");
                entity.Property(e => e.RequiresPrescription).HasColumnName("requires_prescription");
                entity.Property(e => e.SellingPrice).HasColumnType("decimal(18, 2)").HasColumnName("selling_price");
                entity.Property(e => e.ShelfAmount).HasColumnName("shelf_amount");
                entity.Property(e => e.StoredAmount).HasColumnName("stored_amount");
                entity.Property(e => e.SubAmountQuantity).HasColumnName("sub_amount_quantity");
            });

            // --- CRITICAL FIX: Explicit DrugTag Join Entity Configuration ---
            modelBuilder.Entity<DrugTag>(entity =>
            {
                // 1. Map to the correct table name
                entity.ToTable("drug_tags");

                // 2. Define the composite primary key
                entity.HasKey(dt => new { dt.DrugId, dt.TagId }).HasName("PK__drug_tag__07DB5927FFBD29C7");

                // 3. Foreign Key: DrugTag -> Drug
                entity.HasOne(dt => dt.Drug)
                    .WithMany(d => d.DrugTags)
                    .HasForeignKey(dt => dt.DrugId)
                    .HasConstraintName("fk_dt_drug")
                    .OnDelete(DeleteBehavior.Cascade);
                
                // IMPORTANT: Force mapping to snake_case column
                entity.Property(dt => dt.DrugId).HasColumnName("drug_id"); 

                // 4. Foreign Key: DrugTag -> Tag
                entity.HasOne(dt => dt.Tag)
                    .WithMany(t => t.DrugTags)
                    .HasForeignKey(dt => dt.TagId)
                    .HasConstraintName("fk_dt_tag")
                    .OnDelete(DeleteBehavior.Cascade);

                // IMPORTANT: Force mapping to snake_case column
                entity.Property(dt => dt.TagId).HasColumnName("tag_id");
            });
            // ----------------------------------------------------------------

            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.HasKey(e => e.InvoiceId).HasName("PK__invoices__F58DFD4921BD016E");
                entity.ToTable("invoices");
                entity.Property(e => e.InvoiceId).HasColumnName("invoice_id");
                entity.Property(e => e.AdminId).HasColumnName("admin_id");
                entity.Property(e => e.ChangeAmount).HasColumnType("decimal(18, 2)").HasColumnName("change_amount");
                entity.Property(e => e.DiscountAmount).HasColumnType("decimal(18, 2)").HasColumnName("discount_amount");
                entity.Property(e => e.InvoiceTime).HasDefaultValueSql("(sysutcdatetime())").HasColumnName("invoice_time");
                entity.Property(e => e.TaxAmount).HasColumnType("decimal(18, 2)").HasColumnName("tax_amount");
                entity.Property(e => e.AmountPaid).HasColumnType("decimal(18, 2)").HasColumnName("amount_paid");
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)").HasColumnName("total_amount");
                entity.HasOne(d => d.Admin).WithMany(p => p.Invoices)
                    .HasForeignKey(d => d.AdminId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("fk_invoice_admin");
            });

            modelBuilder.Entity<InvoiceItem>(entity =>
            {
                entity.HasKey(e => e.ItemId).HasName("PK__invoice___52020FDDA3592531");
                entity.ToTable("invoice_items");
                entity.Property(e => e.ItemId).HasColumnName("item_id");
                entity.Property(e => e.DrugId).HasColumnName("drug_id");
                entity.Property(e => e.InvoiceId).HasColumnName("invoice_id");
                entity.Property(e => e.Quantity).HasColumnName("quantity");
                entity.HasOne(d => d.Drug).WithMany(p => p.InvoiceItems)
                    .HasForeignKey(d => d.DrugId)
                    .HasConstraintName("fk_ii_drug");
                entity.HasOne(d => d.Invoice).WithMany(p => p.InvoiceItems)
                    .HasForeignKey(d => d.InvoiceId)
                    .HasConstraintName("fk_ii_invoice");
            });

            modelBuilder.Entity<Tag>(entity =>
            {
                entity.HasKey(e => e.TagId).HasName("PK__tags__4296A2B656706952");
                entity.ToTable("tags");
                entity.HasIndex(e => e.Name, "UQ__tags__72E12F1B6BE40C81").IsUnique();
                entity.Property(e => e.TagId).HasColumnName("tag_id");
                entity.Property(e => e.Name).HasMaxLength(100).HasColumnName("name");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
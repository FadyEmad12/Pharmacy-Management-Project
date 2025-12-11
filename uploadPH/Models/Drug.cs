using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema; // <-- ADD THIS USING

namespace Pharmacy.Models;

public partial class Drug
{
    [Column("drug_id")]
    public int DrugId { get; set; }

    [Column("name")]
    public string Name { get; set; } = null!;

    [Column("selling_price", TypeName = "decimal(18, 2)")]
    public decimal SellingPrice { get; set; }

    [Column("purchasing_price", TypeName = "decimal(18, 2)")]
    public decimal PurchasingPrice { get; set; }

    [Column("barcode")]
    public string? Barcode { get; set; }

    [Column("image_url")]
    public string? ImageUrl { get; set; }

    [Column("description_before_use")]
    public string? DescriptionBeforeUse { get; set; }

    [Column("description_how_to_use")]
    public string? DescriptionHowToUse { get; set; }

    [Column("description_side_effects")]
    public string? DescriptionSideEffects { get; set; }

    [Column("requires_prescription")]
    public bool RequiresPrescription { get; set; }

    [Column("drug_type")]
    public string DrugType { get; set; } = null!;

    [Column("manufacturer")]
    public string? Manufacturer { get; set; }

    [Column("expiration_date")]
    public DateOnly? ExpirationDate { get; set; }

    [Column("shelf_amount")]
    public int ShelfAmount { get; set; }

    [Column("stored_amount")]
    public int StoredAmount { get; set; }

    [Column("low_amount")]
    public int LowAmount { get; set; }

    [Column("sub_amount_quantity")]
    public int SubAmountQuantity { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }


    // NAVIGATION PROPERTIES (Relationships)

    // One-to-Many
    public virtual ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();

    // DELETE THIS LINE! This is the old, confusing, implicit M:N property.
    // public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();


    // KEEP THIS LINE: This is the correct, explicit M:N property using the join entity.
    public virtual ICollection<DrugTag> DrugTags { get; set; } = new List<DrugTag>();
}
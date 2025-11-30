using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Pharmacy.Models;

[Table("drugs")]
[Index("IsLow", Name = "IX_drugs_is_low")]
[Index("Barcode", Name = "UQ__drugs__C16E36F83291922D", IsUnique = true)]
public partial class Drug
{
    [Key]
    [Column("drug_id")]
    public int DrugId { get; set; }

    [Column("name")]
    [StringLength(255)]
    public string Name { get; set; } = null!;

    [Column("price", TypeName = "decimal(18, 2)")]
    public decimal Price { get; set; }

    [Column("barcode")]
    [StringLength(255)]
    public string? Barcode { get; set; }

    [Column("shelf_amount")]
    public int ShelfAmount { get; set; }

    [Column("stored_amount")]
    public int StoredAmount { get; set; }

    [Column("type_quantity")]
    [StringLength(10)]
    public string TypeQuantity { get; set; } = null!;

    [Column("amount_per_sub")]
    public int AmountPerSub { get; set; }

    [Column("amount_per_sub_left")]
    public int AmountPerSubLeft { get; set; }

    [Column("low_threshold")]
    public int LowThreshold { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("total_in_sub")]
    public int? TotalInSub { get; set; }

    [Column("is_low")]
    public bool? IsLow { get; set; }

    [InverseProperty("Drug")]
    public virtual ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();

    [ForeignKey("DrugId")]
    [InverseProperty("Drugs")]
    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
}

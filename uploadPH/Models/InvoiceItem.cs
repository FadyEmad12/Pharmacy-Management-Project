using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Pharmacy.Models;

[Table("invoice_items")]
public partial class InvoiceItem
{
    [Key]
    [Column("item_id")]
    public int ItemId { get; set; }

    [Column("invoice_id")]
    public int InvoiceId { get; set; }

    [Column("drug_id")]
    public int DrugId { get; set; }

    [Column("quantity")]
    public int Quantity { get; set; }

    [Column("unit_price", TypeName = "decimal(18, 2)")]
    public decimal UnitPrice { get; set; }

    [Column("unit_type_sold")]
    [StringLength(10)]
    public string UnitTypeSold { get; set; } = null!;

    [Column("total_price", TypeName = "decimal(29, 2)")]
    public decimal? TotalPrice { get; set; }

    [ForeignKey("DrugId")]
    [InverseProperty("InvoiceItems")]
    public virtual Drug Drug { get; set; } = null!;

    [ForeignKey("InvoiceId")]
    [InverseProperty("InvoiceItems")]
    public virtual Invoice Invoice { get; set; } = null!;
}

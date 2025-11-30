using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Pharmacy.Models;

[Table("invoices")]
public partial class Invoice
{
    [Key]
    [Column("invoice_id")]
    public int InvoiceId { get; set; }

    [Column("admin_id")]
    public int? AdminId { get; set; }

    [Column("invoice_time")]
    public DateTime InvoiceTime { get; set; }

    [ForeignKey("AdminId")]
    [InverseProperty("Invoices")]
    public virtual Admin? Admin { get; set; }

    [InverseProperty("Invoice")]
    public virtual ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
}

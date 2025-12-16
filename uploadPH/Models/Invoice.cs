using System;
using System.Collections.Generic;

namespace Pharmacy.Models;

public partial class Invoice
{
    public int InvoiceId { get; set; }

    public int? AdminId { get; set; }

    public DateTime InvoiceTime { get; set; }

    public decimal TotalAmount { get; set; }

    public decimal TaxAmount { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal AmountPaid { get; set; } 

    public decimal ChangeAmount { get; set; }

    public virtual Admin? Admin { get; set; }

    public virtual ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
}

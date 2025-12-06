using System;
using System.Collections.Generic;

namespace Pharmacy.Models;

public partial class InvoiceItem
{
    public int ItemId { get; set; }

    public int InvoiceId { get; set; }

    public int DrugId { get; set; }

    public int Quantity { get; set; }

    public virtual Drug Drug { get; set; } = null!;

    public virtual Invoice Invoice { get; set; } = null!;
}

namespace Pharmacy.Models.Dto
{
    public class InvoiceSummaryDto
    {
        public int InvoiceId { get; set; }
        public int? AdminId { get; set; }
        public string? AdminUsername { get; set; }
        public DateTime InvoiceTime { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TaxAmount { get; set; }
         public decimal ChangeAmount { get; set; }
        public decimal DiscountAmount { get; set; }
    }
}

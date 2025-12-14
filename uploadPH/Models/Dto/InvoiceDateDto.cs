namespace Pharmacy.Models.Dto
{
    public class InvoiceDateDto
    {
        public int InvoiceId { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime InvoiceTime { get; set; }
    }
}

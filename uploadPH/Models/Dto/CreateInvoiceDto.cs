namespace Pharmacy.Models.Dto
{
    public class CreateInvoiceDto
    {
        public int AdminId { get; set; }
        public List<InvoiceItemDto> Items { get; set; } = new List<InvoiceItemDto>();
        public decimal DiscountAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal AmountPaid { get; set; }
    }
}

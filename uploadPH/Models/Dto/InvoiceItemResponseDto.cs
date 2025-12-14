namespace Pharmacy.Models.Dto
{
    public class InvoiceItemResponseDto
    {
        public int ItemId { get; set; }
        public int DrugId { get; set; }
        public int Quantity { get; set; }
        public decimal PricePerUnit { get; set; }
        public decimal TotalPrice { get; set; }
    }
}

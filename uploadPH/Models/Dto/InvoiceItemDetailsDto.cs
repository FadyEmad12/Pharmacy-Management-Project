namespace Pharmacy.Models.Dto
{
    public class InvoiceItemDetailsDto
    {
        public int ItemId { get; set; }
        public int DrugId { get; set; }
        public string DrugName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal PricePerUnit { get; set; }
        public decimal TotalPrice { get; set; }
    }
}

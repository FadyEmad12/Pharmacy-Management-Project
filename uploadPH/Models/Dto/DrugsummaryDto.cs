namespace Pharmacy.Models.Dto
{
    public class DrugsummaryDto
    {
        public int DrugId { get; set; }
        public string Name { get; set; } = null!;
        public decimal SellingPrice { get; set; }
        public int ShelfAmount { get; set; }
    }
}

namespace Pharmacy.Models.Dto
{
    public class DrugsummaryDto
    {
       public int DrugId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int ShelfAmount { get; set; }  // amount
    }
}

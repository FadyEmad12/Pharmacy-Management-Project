namespace Pharmacy.Models.Dto
{
    public class DrugReadDto
    {
        public int DrugId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string? Barcode { get; set; }

        public int ShelfAmount { get; set; }
        public int StoredAmount { get; set; }

        public int TotalInSub { get; set; }
        public bool IsLow { get; set; }
    }
}

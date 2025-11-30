namespace Pharmacy.Models.Dto
{
    public class DrugDto
    {
        public int DrugId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Barcode { get; set; }
        public int ShelfAmount { get; set; }
        public int StoredAmount { get; set; }
        public string TypeQuantity { get; set; }
        public int AmountPerSub { get; set; }
        public int AmountPerSubLeft { get; set; }
        public int LowThreshold { get; set; }
        public int TotalInSub { get; set; }
        public bool IsLow { get; set; }
    }
}

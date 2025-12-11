namespace Pharmacy.Dtos
{
    public class DrugCreateDto
    {
        public string Name { get; set; } = null!;
        public decimal SellingPrice { get; set; }
        public decimal PurchasingPrice { get; set; }
        public string? Barcode { get; set; }
        
        // 
        public IFormFile? Image { get; set; } = null!; 
        
        public string? DescriptionBeforeUse { get; set; }
        public string? DescriptionHowToUse { get; set; }
        public string? DescriptionSideEffects { get; set; }
        public bool RequiresPrescription { get; set; }
        public string DrugType { get; set; } = null!;
        public string? Manufacturer { get; set; }
        public DateOnly? ExpirationDate { get; set; }
        public int ShelfAmount { get; set; }
        public int StoredAmount { get; set; }
        public int LowAmount { get; set; }
        public int SubAmountQuantity { get; set; }

        public List<string> Tags { get; set; } = new(); 
    }
}
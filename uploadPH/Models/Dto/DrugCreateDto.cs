namespace Pharmacy.Dtos
{
    public class DrugCreateDto
    {
        // All core drug properties from the HTML form
        public string Name { get; set; } = null!;
        public decimal SellingPrice { get; set; }
        public decimal PurchasingPrice { get; set; }
        public string? Barcode { get; set; }
        
        // This receives the uploaded file content
        public IFormFile Image { get; set; } = null!; // IMPORTANT: Changed to be NOT nullable based on your validation
        
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

        // New property to receive a list of tag names from the form/request body
        public List<string> Tags { get; set; } = new(); 
    }
}
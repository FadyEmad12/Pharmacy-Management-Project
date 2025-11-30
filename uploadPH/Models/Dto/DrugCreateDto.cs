using System.ComponentModel.DataAnnotations;

namespace Pharmacy.Models.Dto
{
    public class DrugCreateDto
    {
        [Required]
        public string Name { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        public string Barcode { get; set; }

        [Range(0, int.MaxValue)]
        public int ShelfAmount { get; set; }

        [Range(0, int.MaxValue)]
        public int StoredAmount { get; set; }

        [Required]
        [RegularExpression("whole|sub")]
        public string TypeQuantity { get; set; }

        [Range(1, int.MaxValue)]
        public int AmountPerSub { get; set; }

        [Range(0, int.MaxValue)]
        public int AmountPerSubLeft { get; set; }

        [Range(1, int.MaxValue)]
        public int LowThreshold { get; set; }
    }
}

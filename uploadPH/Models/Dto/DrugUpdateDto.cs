using System.ComponentModel.DataAnnotations;

namespace Pharmacy.Models.Dto
{
    public class DrugUpdateDto
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Barcode { get; set; }
        public int ShelfAmount { get; set; }
        public int StoredAmount { get; set; }
        [RegularExpression("whole|sub")]
        public string TypeQuantity { get; set; }
        public int AmountPerSub { get; set; }
        public int AmountPerSubLeft { get; set; }
        public int LowThreshold { get; set; }
    }
}

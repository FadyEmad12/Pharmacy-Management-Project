// In Pharmacy.Dtos/DrugUpdateDto.cs

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Pharmacy.Dtos
{
    public class DrugUpdateDto
    {
        [Required]
        public string Name { get; set; } = null!;
        
        [Required]
        public decimal SellingPrice { get; set; }
        
        [Required]
        public decimal PurchasingPrice { get; set; }
        
        public string? Barcode { get; set; }
        
        public string? DescriptionBeforeUse { get; set; }
        public string? DescriptionHowToUse { get; set; }
        public string? DescriptionSideEffects { get; set; }
        
        public bool RequiresPrescription { get; set; }
        
        [Required]
        public string DrugType { get; set; } = null!;
        
        public string? Manufacturer { get; set; }
        public DateOnly? ExpirationDate { get; set; }
        
        public int ShelfAmount { get; set; }
        public int StoredAmount { get; set; }
        public int LowAmount { get; set; }
        public int SubAmountQuantity { get; set; }

        public List<string>? Tags { get; set; }
    }
}
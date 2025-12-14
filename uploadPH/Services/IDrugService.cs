using Pharmacy.Dtos;
using Pharmacy.Models.Dto;

namespace Pharmacy.Services
{
    public interface IDrugService
    {
        Task<List<DrugsummaryDto>> GetAllDrugsAsync();
        Task<DrugsummaryDto?> GetDrugByIdAsync(int id);
        Task<List<DrugsummaryDto>> SearchDrugsAsync(string? name, string? barcode, string? type, string? tags);
        Task<DrugCreated> CreateDrugAsync(DrugCreateDto dto, string image);
        Task<bool> UpdateDrugAsync(int id, DrugUpdateDto dto);
        Task<bool> DeleteDrugAsync(int id);
        Task<string> GenerateUniqueBarcodeAsync();
    }
}

using Pharmacy.Models;
using Pharmacy.Models.Dto;

namespace Pharmacy.Services
{
    public interface IInvoiceService
    {
        Task<InvoiceResponseDto> CreateInvoiceAsync(CreateInvoiceDto dto);
        Task<List<InvoiceSummaryDto>> GetAllInvoicesAsync();
        Task<InvoiceDetailsDto> GetInvoiceByIdAsync(int invoiceId);
        Task<List<InvoiceDateDto>> GetInvoicesByDateRangeAsync(DateTime from, DateTime to);
        Task DeleteInvoiceAsync(int invoiceId);
        Task<List<InvoiceItemDetailsDto>> GetItemsByInvoiceIdAsync(int invoiceId);
        Task RemoveInvoiceItemAsync(int itemId, int quantityToRemove = 1);
        Task<InvoiceItemResponseDto> AddItemToInvoiceAsync(int invoiceId, InvoiceItemDto dto);





    }
}

using Pharmacy.Models;
using Pharmacy.Models.Dto;

namespace Pharmacy.Repository
{
    public interface IInvoiceItemRepository
    {
        Task<InvoiceItem> AddAsync(InvoiceItem item);
        Task SaveChangesAsync();
        Task<List<InvoiceItem>> GetItemsByInvoiceIdAsync(int invoiceId);
        Task<InvoiceItem?> GetByIdAsync(int itemId);

        Task<bool> ExistsAsync(int invoiceId);
        Task DeleteAsync(InvoiceItem item);
        
    }
}

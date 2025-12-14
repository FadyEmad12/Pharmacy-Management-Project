using Microsoft.EntityFrameworkCore;
using Pharmacy.Models;

namespace Pharmacy.Repository
{
    public interface IInvoiceRepository
    {
        Task AddAsync(Invoice invoice);
        Task<Invoice?> GetByIdAsync(int invoiceId);
        Task SaveChangesAsync();
        Task<List<Invoice>> GetAllAsync();
        Task<List<Invoice>> GetByDateRangeAsync(DateTime from, DateTime to);
        Task<Invoice?> GetByIdForDeleteAsync(int invoiceId);
        Task DeleteAsync(Invoice invoice);
        Task<Invoice?> GetByIdWithItemsAsync(int invoiceId);



    }
}

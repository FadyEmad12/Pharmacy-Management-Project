using Microsoft.EntityFrameworkCore;
using Pharmacy.Data;
using Pharmacy.Models;

namespace Pharmacy.Repository
{
    public class InvoiceItemRepository:IInvoiceItemRepository
    {
        private readonly PharmacyDbContext _context;

        public InvoiceItemRepository(PharmacyDbContext context)
        {
            _context = context;
        }

       public async Task<InvoiceItem> AddAsync(InvoiceItem item)
        {
            await _context.InvoiceItems.AddAsync(item);
            return item;
        }



        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task<List<InvoiceItem>> GetItemsByInvoiceIdAsync(int invoiceId)
        {
            return await _context.InvoiceItems
                .Include(ii => ii.Drug)
                .Where(ii => ii.InvoiceId == invoiceId)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<bool> ExistsAsync(int invoiceId)
        {
            return await _context.Invoices.AnyAsync(i => i.InvoiceId == invoiceId);
        }
        public Task DeleteAsync(InvoiceItem item)
        {
            _context.InvoiceItems.Remove(item);
            return Task.CompletedTask;
        }
        public async Task<InvoiceItem?> GetByIdAsync(int itemId)
        {
            return await _context.InvoiceItems
                .FirstOrDefaultAsync(i => i.ItemId == itemId);
        }
    }
}

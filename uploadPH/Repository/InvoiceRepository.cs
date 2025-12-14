using Microsoft.EntityFrameworkCore;
using Pharmacy.Data;
using Pharmacy.Models;

namespace Pharmacy.Repository
{
    public class InvoiceRepository:IInvoiceRepository
    {
        private readonly PharmacyDbContext _context;

        public InvoiceRepository(PharmacyDbContext context)
        {
            _context = context;
        } 
        public async Task AddAsync(Invoice invoice)
        {
            await _context.Invoices.AddAsync(invoice);
        }


        public async Task<Invoice?> GetByIdAsync(int invoiceId)
        {
            return await _context.Invoices
                .Include(i => i.Admin)
                .Include(i => i.InvoiceItems)
                    .ThenInclude(ii => ii.Drug)
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task<List<Invoice>> GetAllAsync()
        {
            return await _context.Invoices
                .Include(i => i.Admin)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<List<Invoice>> GetByDateRangeAsync(DateTime from, DateTime to)
        {
            return await _context.Invoices
                .Where(i => i.InvoiceTime >= from && i.InvoiceTime <= to)
                .AsNoTracking()
                .OrderBy(i => i.InvoiceTime)
                .ToListAsync();
        }
        public async Task<Invoice?> GetByIdForDeleteAsync(int invoiceId)
        {
            return await _context.Invoices
                .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);
        }

        public Task DeleteAsync(Invoice invoice)
        {
            _context.Invoices.Remove(invoice);
            return Task.CompletedTask;
        }
        public async Task<Invoice?> GetByIdWithItemsAsync(int invoiceId)
        {
            return await _context.Invoices
                .Include(i => i.InvoiceItems)
                    .ThenInclude(ii => ii.Drug)
                .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);
        }


    }
}

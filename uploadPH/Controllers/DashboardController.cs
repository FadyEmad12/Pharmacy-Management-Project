using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pharmacy.Data;
using Pharmacy.Dtos;

namespace Pharmacy.Controllers
{
    [ApiController]
    [Route("api/dashboard")]
    public class DashboardController : ControllerBase
    {
        private readonly PharmacyDbContext _context;

        public DashboardController(PharmacyDbContext context)
        {
            _context = context;
        }

        [HttpGet("cards")]
        [Authorize(Roles = "super_admin")]
        public async Task<IActionResult> GetDashboardCards()
        {
            var now = DateTime.UtcNow;

            var today = DateOnly.FromDateTime(now);
            var oneMonthFromNow = today.AddMonths(1);

            // 1️⃣ Near expiration (STORAGE amount)
            var nearExpirationStorageAmount = await _context.Drugs
                .AsNoTracking()
                .Where(d =>
                    d.ExpirationDate != null &&
                    d.ExpirationDate <= oneMonthFromNow &&
                    d.StoredAmount > 0)
                .SumAsync(d => (int?)d.StoredAmount) ?? 0;

            // 2️⃣ Monthly income
            var monthlyIncome = await _context.Invoices
                .AsNoTracking()
                .Where(i =>
                    i.InvoiceTime.Year == now.Year &&
                    i.InvoiceTime.Month == now.Month)
                .SumAsync(i => (decimal?)i.TotalAmount) ?? 0;

            // 3️⃣ Annual income
            var annualIncome = await _context.Invoices
                .AsNoTracking()
                .Where(i => i.InvoiceTime.Year == now.Year)
                .SumAsync(i => (decimal?)i.TotalAmount) ?? 0;

            // 4️⃣ Total drugs in STORAGE
            var totalDrugsInStorage = await _context.Drugs
                .AsNoTracking()
                .SumAsync(d => (int?)d.StoredAmount) ?? 0;

            return Ok(new
            {
                nearExpirationStorageAmount,
                monthlyIncome,
                annualIncome,
                totalDrugsInStorage
            });
        }

        [HttpGet("low-stock")]
        [Authorize(Roles = "super_admin")]
        public async Task<IActionResult> GetLowStockDrugs()
        {
            var lowStockDrugs = await _context.Drugs
                .AsNoTracking()
                .Where(d =>
                    d.ShelfAmount <= d.LowAmount &&
                    d.ShelfAmount > 0) 
                .OrderBy(d => d.ShelfAmount)
                .ToListAsync();

            return Ok(lowStockDrugs);
        }




        [HttpGet("most-sold")]
        [Authorize(Roles = "super_admin")]
        public async Task<IActionResult> GetMostSoldDrugsLastMonth()
        {
            var fromDate = DateTime.UtcNow.AddMonths(-1);

            var result = await _context.InvoiceItems
                .AsNoTracking()
                .Where(ii => ii.Invoice.InvoiceTime >= fromDate)
                .GroupBy(ii => ii.Drug)
                .Select(g => new
                {
                    DrugId = g.Key.DrugId,
                    Name = g.Key.Name,
                    Barcode = g.Key.Barcode,
                    DrugType = g.Key.DrugType,
                    Manufacturer = g.Key.Manufacturer,

                    TotalQuantitySold = g.Sum(x => x.Quantity),

                    TotalRevenue = g.Sum(x => x.Quantity * g.Key.SellingPrice)
                })
                .OrderByDescending(x => x.TotalQuantitySold)
                .ToListAsync();

            return Ok(result);
        }
    }
}

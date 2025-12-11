using Pharmacy.Data;
using Pharmacy.Models;

namespace Pharmacy.Services
{
    public interface IAdminLogService
    {
        Task LogAsync(int adminId, string actionType);
    }

    public class AdminLogService : IAdminLogService
    {
        private readonly PharmacyDbContext _context;

        public AdminLogService(PharmacyDbContext context)
        {
            _context = context;
        }

        public async Task LogAsync(int adminId, string actionType)
        {
            var log = new AdminLog
            {
                AdminId = adminId,
                ActionType = actionType,
                ActionTime = DateTime.UtcNow
            };

            await _context.AdminLogs.AddAsync(log);
            await _context.SaveChangesAsync();
        }
    }
}

using Pharmacy.Repository;
using Pharmacy.Models;
using Pharmacy.Services;

namespace Pharmacy.Services
{
    public class DatabaseSeeder
    {
        private readonly IAdminRepository _adminRepo;
        private readonly IPasswordHelper _passwordHelper;

        public DatabaseSeeder(IAdminRepository adminRepo, IPasswordHelper passwordHelper)
        {
            _adminRepo = adminRepo;
            _passwordHelper = passwordHelper;
        }

        public void Seed()
        {
            // Check if super admin already exists
            if (_adminRepo.GetByEmail("admin@example.com") != null)
                return; // Already seeded

            var admin = new Admin
            {
                Username = "admin",
                Email = "admin@example.com",
                PasswordHash = _passwordHelper.HashPassword("admin"),
                Role = "super_admin",
                CreatedAt = DateTime.UtcNow
            };

            _adminRepo.Add(admin);
            _adminRepo.Save();
        }
    }
}

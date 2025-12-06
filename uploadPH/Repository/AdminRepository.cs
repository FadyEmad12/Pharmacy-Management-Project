// In Pharmacy.Repository/AdminRepository.cs
 //
using Pharmacy.Data;
using Pharmacy.Models;
using Microsoft.EntityFrameworkCore;

namespace Pharmacy.Repository
{
    public class AdminRepository : IAdminRepository
    {
        private readonly PharmacyDbContext _context;

        public AdminRepository(PharmacyDbContext context)
        {
            _context = context;
        }

        public Admin? GetByEmail(string email)
        {
            return _context.Admins.FirstOrDefault(a => a.Email == email);
        }

        public List<Admin> GetAll()
        {
            return _context.Admins.ToList();
        }

        public void Add(Admin admin)
        {
            _context.Admins.Add(admin);
        }

        public void Update(Admin admin)
        {
            _context.Admins.Update(admin);
        }

        public void Delete(int id)
        {
            var admin = _context.Admins.Find(id);
            if (admin != null)
            {
                _context.Admins.Remove(admin);
            }
        }

        public void Save()
        {
            _context.SaveChanges();
        }
        
        public void AddLog(AdminLog log)
        {
            _context.AdminLogs.Add(log);
        }
    }
}
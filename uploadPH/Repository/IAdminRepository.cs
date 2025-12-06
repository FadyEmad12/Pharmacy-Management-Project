// In Pharmacy.Repository/IAdminRepository.cs

using Pharmacy.Models;

namespace Pharmacy.Repository
{
    public interface IAdminRepository
    {
        Admin? GetByEmail(string email);
        List<Admin> GetAll();
        void Add(Admin admin);
        void Update(Admin admin);
        void Delete(int id);
        void Save();
        
        // Logs
        void AddLog(AdminLog log);
    }
}
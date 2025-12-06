
using BCrypt.Net; 
using Pharmacy.Services; 

namespace Pharmacy.Services
{
    public class PasswordHelper : IPasswordHelper
    {
        public string HashPassword(string password)
        {
            // Now BCrypt.Net should be found
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}
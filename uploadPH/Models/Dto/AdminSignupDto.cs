using System.ComponentModel.DataAnnotations;

namespace Pharmacy.Dtos
{
    public class AdminSignupDto
    {
        [Required]
        public string Username { get; set; } = null!;
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
        
        [Required]
        [MinLength(6)]
        public string Password { get; set; } = null!;

        [Required]
        public string Role { get; set; } = null!;
    }
}
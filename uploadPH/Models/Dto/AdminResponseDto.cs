// In Pharmacy.Dtos/AdminResponseDto.cs

namespace Pharmacy.Dtos
{
    public class AdminResponseDto
    {
        public int AdminId { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
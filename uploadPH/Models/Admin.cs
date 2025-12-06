// In Pharmacy.Models/Admin.cs

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pharmacy.Models
{
    [Table("admins")]
    public class Admin
    {
        [Key]
        [Column("admin_id")]
        public int AdminId { get; set; }

        [Column("username")]
        [Required]
        public string Username { get; set; } = null!;

        [Column("email")]
        [Required]
        public string Email { get; set; } = null!;

        [Column("password_hash")]
        [Required]
        public string PasswordHash { get; set; } = null!;

        [Column("role")]
        [Required]
        public string Role { get; set; } = null!; // e.g., "super_admin" or "cashier"

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        // Navigation Property for AdminLogs
        public virtual ICollection<AdminLog> AdminLogs { get; set; } = new List<AdminLog>();
        public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pharmacy.Models
{
    [Table("admin_logs")]
    public class AdminLog
    {
        [Key]
        [Column("log_id")]
        public int LogId { get; set; }

        [Column("admin_id")]
        public int AdminId { get; set; }

        [Column("action_type")]
        [Required]
        public string ActionType { get; set; } = null!; // "login" or "logout"

        [Column("action_time")]
        public DateTime ActionTime { get; set; }

        // Navigation Property to Admin
        public virtual Admin Admin { get; set; } = null!;
    }
}
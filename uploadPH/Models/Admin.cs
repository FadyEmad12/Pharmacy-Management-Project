using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Pharmacy.Models;

[Table("admins")]
[Index("Email", Name = "UQ__admins__AB6E6164868C60A7", IsUnique = true)]
[Index("Username", Name = "UQ__admins__F3DBC57251ED6670", IsUnique = true)]
public partial class Admin
{
    [Key]
    [Column("admin_id")]
    public int AdminId { get; set; }

    [Column("username")]
    [StringLength(255)]
    public string Username { get; set; } = null!;

    [Column("email")]
    [StringLength(255)]
    public string Email { get; set; } = null!;

    [Column("password_hash")]
    [StringLength(512)]
    public string PasswordHash { get; set; } = null!;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [InverseProperty("Admin")]
    public virtual ICollection<AdminLog> AdminLogs { get; set; } = new List<AdminLog>();

    [InverseProperty("Admin")]
    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Pharmacy.Models;

[Table("admin_logs")]
public partial class AdminLog
{
    [Key]
    [Column("log_id")]
    public int LogId { get; set; }

    [Column("admin_id")]
    public int AdminId { get; set; }

    [Column("action_type")]
    [StringLength(50)]
    public string ActionType { get; set; } = null!;

    [Column("action_time")]
    public DateTime ActionTime { get; set; }

    [ForeignKey("AdminId")]
    [InverseProperty("AdminLogs")]
    public virtual Admin Admin { get; set; } = null!;
}

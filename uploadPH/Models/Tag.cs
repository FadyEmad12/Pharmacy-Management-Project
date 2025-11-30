using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Pharmacy.Models;

[Table("tags")]
[Index("Name", Name = "UQ__tags__72E12F1B3208760B", IsUnique = true)]
public partial class Tag
{
    [Key]
    [Column("tag_id")]
    public int TagId { get; set; }

    [Column("name")]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    [ForeignKey("TagId")]
    [InverseProperty("Tags")]
    public virtual ICollection<Drug> Drugs { get; set; } = new List<Drug>();
}

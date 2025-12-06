using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema; 

namespace Pharmacy.Models;

public partial class Tag
{
    // Fix 1: Explicitly map the Primary Key to 'tag_id'
    [Column("tag_id")]
    public int TagId { get; set; }

    // Fix 2: Explicitly map the Name property to 'name'
    [Column("name")]
    public string Name { get; set; } = null!;

    // This property implicitly relies on the DrugTag configuration to know the DB column names
    public virtual ICollection<DrugTag> DrugTags { get; set; } = new List<DrugTag>();
}
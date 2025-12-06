using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Pharmacy.Models
{
    // Corresponds to the 'drug_tags' table
    public class DrugTag
    {
        // 🔑 PRIMARY KEY (Composite Key Configuration)
        // Must match the column names defined in your PharmacyDbContext

        [Column("drug_id")] // Maps C# DrugId to SQL drug_id
        public int DrugId { get; set; }

        [Column("tag_id")] // Maps C# TagId to SQL tag_id
        public int TagId { get; set; }

        // Navigation Properties
        public virtual Drug Drug { get; set; } = null!;
        public virtual Tag Tag { get; set; } = null!;
    }
}
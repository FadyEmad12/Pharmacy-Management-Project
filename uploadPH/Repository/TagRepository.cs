using Microsoft.EntityFrameworkCore;
using Pharmacy.Data;
using Pharmacy.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pharmacy.Repository
{
    // The TagRepository implements the ITagRepository interface
    public class TagRepository : ITagRepository
    {
        private readonly PharmacyDbContext _context;

        // Constructor for Dependency Injection (DI)
        public TagRepository(PharmacyDbContext context)
        {
            _context = context;
        }

        public async Task<List<Tag>> GetTagsByNamesAsync(List<string> names)
        {
            var lowerCaseNames = names.Select(n => n.ToLower()).ToList();

            return await _context.Tags
                .Where(t => lowerCaseNames.Contains(t.Name.ToLower()))
                .AsNoTracking() 
                .ToListAsync();;
        }

        public void AddRange(IEnumerable<Tag> tags)
        {
            _context.Tags.AddRange(tags);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}

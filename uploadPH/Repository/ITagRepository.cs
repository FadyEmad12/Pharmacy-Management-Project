using Pharmacy.Models; // <-- ADD THIS LINE

namespace Pharmacy.Repository
{
    public interface ITagRepository
    {
        Task<List<Tag>> GetTagsByNamesAsync(List<string> names);
        void AddRange(IEnumerable<Tag> tags);
        Task SaveAsync();
    }
}
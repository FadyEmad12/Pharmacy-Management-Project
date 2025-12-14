using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pharmacy.Models;
namespace Pharmacy.Repository
{
    public interface IDrugRepository
    {
        // Get all drugs
        Task<List<Drug>> GetAllAsync();

        // Get a single drug by ID
        Task<Drug?> GetByIdAsync(int id);

        // Get a drug by ID including its tags
        Task<Drug?> GetByIdWithTagsAsync(int id);

        // Search drugs with optional filters
        Task<List<Drug>> SearchAsync(string? name, string? barcode, string? type, List<string>? tags);

        // Filter queryable by individual properties
        IQueryable<Drug> FilterByName(IQueryable<Drug> query, string name);
        IQueryable<Drug> FilterByBarcode(IQueryable<Drug> query, string barcode);
        IQueryable<Drug> FilterByType(IQueryable<Drug> query, string type);
        IQueryable<Drug> FilterByTags(IQueryable<Drug> query, List<string> tags);

        // Search by name only
        Task<List<Drug>> SearchByNameAsync(string name);

        // Add a new drug
        Task AddAsync(Drug drug);

        // Save changes to database
        Task SaveChangesAsync();

        // Update an existing drug
        Task UpdateAsync(Drug drug);

        // Delete a drug by ID
        Task DeleteAsync(int id);

        // Tag-related methods
        Task<Tag> CreateTagAsync(string name);
        Task<Tag?> GetTagByNameAsync(string name);
        Task<List<Tag>> GetTagsByNamesAsync(List<string> names);
        Task<bool> BarcodeExistsAsync(string barcode);
    }
}

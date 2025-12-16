using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pharmacy.Data;
using Pharmacy.Models;

namespace Pharmacy.Repository
{
    public class DrugRepository : IDrugRepository
    {
        private readonly PharmacyDbContext _context;

        public DrugRepository(PharmacyDbContext context)
        {
            _context = context;
        }

        public async Task<List<Drug>> GetAllAsync()
        {
            return await _context.Drugs
                .Include(d => d.DrugTags)
                    .ThenInclude(dt => dt.Tag)
                .ToListAsync();
        }

        public async Task<Drug?> GetByIdAsync(int id)
        {
            return await _context.Drugs
                .Include(d => d.DrugTags)
                    .ThenInclude(dt => dt.Tag)
                .FirstOrDefaultAsync(d => d.DrugId == id);
        }

        public async Task<Drug?> GetByIdWithTagsAsync(int id)
        {
            return await _context.Drugs
                .Include(d => d.DrugTags)
                    .ThenInclude(dt => dt.Tag)
                .FirstOrDefaultAsync(d => d.DrugId == id);
        }

        public async Task<Drug?> GetByBarcodeAsync(string barcode)
        {
            return await _context.Drugs
                .Include(d => d.DrugTags)
                    .ThenInclude(dt => dt.Tag)
                .FirstOrDefaultAsync(d => d.Barcode == barcode);
        }

        public async Task<List<Drug>> SearchByNameAsync(string name)
        {
            return await _context.Drugs
                .Where(d => d.Name.Contains(name))
                .ToListAsync();
        }

        public IQueryable<Drug> FilterByName(IQueryable<Drug> query, string name)
        {
            return query.Where(d => d.Name.Contains(name));
        }

      public IQueryable<Drug> FilterByBarcode(IQueryable<Drug> query, string barcode)
        {
            return query.Where(d =>
                d.Barcode != null &&
                d.Barcode.Contains(barcode)
            );
        }


        public IQueryable<Drug> FilterByType(IQueryable<Drug> query, string type)
        {
            return query.Where(d => d.DrugType == type);
        }


        public IQueryable<Drug> FilterByTags(IQueryable<Drug> query, List<string> tags)
        {
            return query.Where(d =>
                d.DrugTags.Any(dt =>
                    dt.Tag != null &&
                    dt.Tag.Name != null &&
                    tags.Contains(dt.Tag.Name)
                )
            );
        }


        public async Task<List<Drug>> SearchAsync(string? name, string? barcode, string? type, List<string>? tags)
        {
            var query = _context.Drugs
                .Include(d => d.DrugTags)
                    .ThenInclude(dt => dt.Tag)
                .AsQueryable();

            if (!string.IsNullOrEmpty(name))
                query = FilterByName(query, name);

            if (!string.IsNullOrEmpty(barcode))
                query = FilterByBarcode(query, barcode);

            if (!string.IsNullOrEmpty(type))
                query = FilterByType(query, type);

            if (tags != null && tags.Count > 0)
                query = FilterByTags(query, tags);

            return await query.ToListAsync();
        }

        public async Task AddAsync(Drug drug)
        {
            if (drug == null)
                throw new ArgumentNullException(nameof(drug));

            await _context.Drugs.AddAsync(drug);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Drug drug)
        {
            _context.Drugs.Update(drug);
            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var drug = await _context.Drugs.FindAsync(id);
            if (drug != null)
            {
                _context.Drugs.Remove(drug);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Tag> CreateTagAsync(string name)
        {
            var tag = new Tag { Name = name };
            await _context.Tags.AddAsync(tag);
            await _context.SaveChangesAsync();
            return tag;
        }

        public async Task<Tag?> GetTagByNameAsync(string name)
        {
            return await _context.Tags.FirstOrDefaultAsync(t => t.Name == name);
        }

        public async Task<List<Tag>> GetTagsByNamesAsync(List<string> names)
        {
            var loweredNames = names
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .Select(n => n.ToLower())
                .ToList();

            return await _context.Tags
                .Where(t => t.Name != null && loweredNames.Contains(t.Name.ToLower()))
                .ToListAsync();
        }
        public async Task<bool> BarcodeExistsAsync(string barcode)
        {
            return await _context.Drugs.AnyAsync(d => d.Barcode == barcode);
        }
    }

}

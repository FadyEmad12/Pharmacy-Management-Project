using Microsoft.AspNetCore.Mvc;
using Pharmacy.Models;
using System.Collections.Generic;
using System.Linq;

namespace Pharmacy.Repository
{
    public class DrugRepository : IDrugRepository
    {
        private readonly PharmacyDbContext _context;

        public DrugRepository(PharmacyDbContext context)
        {
            _context = context;
        }
       
        public List<Drug> GetAll()
        {
            return _context.Drugs.ToList(); 
        }

        public Drug? GetById(int id)
        {
            return _context.Drugs.Find(id); 
        }
        public Drug? GetByBarcode(string barcode)
        {
            return _context.Drugs.FirstOrDefault(d => d.Barcode == barcode);
        }
        public List<Drug> SearchByName(string name)
        {
            return _context.Drugs
                           .Where(d => d.Name.Contains(name))
                           .ToList();
        }

        public void Add(Drug drug)
        {
            _context.Drugs.Add(drug);
            _context.SaveChanges();
        }

        public void Update(Drug drug)
        {
            _context.Drugs.Update(drug);
            _context.SaveChanges();
        }
        public void Delete(int id)
        {
            var drug = _context.Drugs.Find(id);
            if (drug != null)
            {
                _context.Drugs.Remove(drug);
                _context.SaveChanges();
            }
        }
    }
}

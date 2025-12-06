using System.Collections.Generic;
using Pharmacy.Models;

namespace Pharmacy.Repository
{
    public interface IDrugRepository
    {
        List<Drug> GetAll();
        Drug? GetById(int id);
        Drug? GetByBarcode(string barcode);
        List<Drug> SearchByName(string name);
        Drug Add(Drug drug);
        void Save();
        void Update(Drug drug);
        void Delete(int id);
    }
}

using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pharmacy.Models;
namespace Pharmacy.Repository
{
    public interface IDrugRepository
    {
        List<Drug> GetAll();
        Drug? GetById(int id);
        Drug? GetByBarcode(string barcode);
        List<Drug> SearchByName(string name);
        void Add(Drug drug);
        void Update(Drug drug);
        void Delete(int id);
    }
}
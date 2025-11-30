using Pharmacy.Models;
using Pharmacy.Models.Dto;
using Pharmacy.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Linq;


namespace Pharmacy.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DrugsController : ControllerBase
    {
        private readonly IDrugRepository _drugRepo;
        private DrugDto MapToDto(Drug drug)
        {
            return new DrugDto
            {
                DrugId = drug.DrugId,
                Name = drug.Name,
                Price = drug.Price,
                Barcode = drug.Barcode,
                ShelfAmount = drug.ShelfAmount,
                StoredAmount = drug.StoredAmount,
                TypeQuantity = drug.TypeQuantity,
                AmountPerSub = drug.AmountPerSub,
                AmountPerSubLeft = drug.AmountPerSubLeft,
                LowThreshold = drug.LowThreshold,

            };
        }
        private DrugReadDto MapToReadDto(Drug d)
        {
            return new DrugReadDto
            {
                DrugId = d.DrugId,
                Name = d.Name,
                Price = d.Price,
                Barcode = d.Barcode,
                ShelfAmount = d.ShelfAmount,
                StoredAmount = d.StoredAmount,
                TotalInSub = d.TotalInSub ?? 0,
                IsLow = d.IsLow ?? false
            };
        }

        public DrugsController(IDrugRepository drugRepo)
        {
            _drugRepo = drugRepo;
        }

        // GET: api/drugs

        [HttpGet]
        public IActionResult GetAllDrugs()
        {
            var drugs = _drugRepo.GetAll();

            var summary = drugs.Select(d => new DrugsummaryDto
            {
                Name = d.Name,
                Price = d.Price,
                ShelfAmount = d.ShelfAmount,
            }).ToList();

            return Ok(summary);
        }

        // GET: api/drugs/5
        [HttpGet("{id}")]
        public IActionResult GetDrugById(int id)
        {
            var drug = _drugRepo.GetById(id);

            if (drug == null)
                return NotFound(new { message = "Drug not found" });

            var result = new DrugsummaryDto
            {
                Name = drug.Name,
                Price = drug.Price,
                ShelfAmount = drug.ShelfAmount
            };

            return Ok(result);
        }
        [HttpGet("barcode/{barcode}")]
        public IActionResult GetByBarcode(string barcode)
        {
            var drug = _drugRepo.GetByBarcode(barcode);

            if (drug == null)
                return NotFound(new { message = "Drug not found" });

            var result = new DrugsummaryDto
            {
                DrugId = drug.DrugId,
                Name = drug.Name,
                Price = drug.Price,
                ShelfAmount = drug.ShelfAmount
            };
            return Ok(result);
        }
        [HttpGet("search")]
        public IActionResult SearchByName([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest(new { message = "Name query is required" });

            var drugs = _drugRepo.SearchByName(name);

            var result = drugs.Select(d => new DrugsummaryDto
            {
                Name = d.Name,
                Price = d.Price,
                ShelfAmount = d.ShelfAmount,
                DrugId=d.DrugId
            }).ToList();

            return Ok(result);
        }

        [HttpPost]
        public IActionResult AddDrug([FromBody] DrugCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var drug = new Drug
            {
                Name = dto.Name,
                Price = dto.Price,
                Barcode = dto.Barcode,
                ShelfAmount = dto.ShelfAmount,
                StoredAmount = dto.StoredAmount,
                TypeQuantity = dto.TypeQuantity,
                AmountPerSub = dto.AmountPerSub,
                AmountPerSubLeft = dto.AmountPerSubLeft,
                LowThreshold = dto.LowThreshold
            };

            _drugRepo.Add(drug);

            var readDto = new DrugReadDto
            {
                DrugId = drug.DrugId,
                Name = drug.Name,
                Price = drug.Price,
                Barcode = drug.Barcode,
                ShelfAmount = drug.ShelfAmount,
                StoredAmount = drug.StoredAmount,
                TotalInSub = drug.TotalInSub ?? 0,
                IsLow = drug.IsLow ?? false
            };

            return CreatedAtAction(nameof(GetDrugById), new { id = drug.DrugId }, readDto);
        }

        // PUT: api/drugs/5
        [HttpPut("{id}")]
        
        public IActionResult UpdateDrug(int id, [FromBody] DrugUpdateDto dto)
        {
            var existing = _drugRepo.GetById(id);
            if (existing == null)
                return NotFound();
            // properities to update

            existing.Name = dto.Name;
            existing.Price = dto.Price;
            existing.TypeQuantity = dto.TypeQuantity;

            _drugRepo.Update(existing);

            return NoContent();
        }

        // DELETE: api/drugs/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var existing = _drugRepo.GetById(id);
            if (existing == null)
                return NotFound(new { message = "Drug not found" });

            _drugRepo.Delete(id);
            return NoContent();
        }
    }
}

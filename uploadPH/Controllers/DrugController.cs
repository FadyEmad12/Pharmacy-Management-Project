using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pharmacy.Data; // Required for PharmacyDbContext
using Pharmacy.Dtos;
using Pharmacy.Models;
using Pharmacy.Repository;
using Pharmacy.Services;

namespace Pharmacy.Controllers
   
{
    [ApiController]
    [Route("api/[controller]")]
    public class DrugsController : ControllerBase
    {
        private readonly IDrugService _drugService;
       
        private readonly List<string> _allowedExtensions = new() { ".jpg", ".jpeg", ".png" };
        private const long MaxAllowedSize = 100 * 1024 * 1024;

        public DrugsController(IDrugService drugService)
        {
            _drugService = drugService;
            
        }

        // GET: api/drugs
        [HttpGet]
        public async Task<IActionResult> GetAllDrugs()
        {
            var drugs = await _drugService.GetAllDrugsAsync();
            return Ok(drugs);
        }

        // GET: api/drugs/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDrugById(int id)
        {
            var drug = await _drugService.GetDrugByIdAsync(id);
            if (drug == null)
                return NotFound(new { message = "Drug not found" });
            return Ok(drug);
        }

        // GET: api/drugs/search
        [HttpGet("search")]
        public async Task<IActionResult> SearchDrugs(string? name, string? barcode, string? type, string? tags)
        {
            var drugs = await _drugService.SearchDrugsAsync(name, barcode, type, tags);
            return Ok(drugs);
        }

        [HttpPost]
        [Authorize(Roles = "super_admin")]
        public async Task<IActionResult> AddDrug([FromForm] DrugCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto.Image == null)
                return BadRequest("Image is required");

            var allowedExtensions = new List<string> { ".jpg", ".jpeg", ".png" };
            var ext = Path.GetExtension(dto.Image.FileName).ToLower();
            if (!allowedExtensions.Contains(ext))
                return BadRequest("Only .jpg, .jpeg, and .png images are allowed!");

            const long maxSize = 100 * 1024 * 1024;
            if (dto.Image.Length > maxSize)
                return BadRequest("Max allowed size for image is 100MB");
            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsPath))
                Directory.CreateDirectory(uploadsPath);

            var fileName = $"{Guid.NewGuid()}_{dto.Image.FileName}";
            var filePath = Path.Combine(uploadsPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.Image.CopyToAsync(stream);
            }
           

            var imageUrl = $"/uploads/{fileName}";


            var result = await _drugService.CreateDrugAsync(dto, imageUrl);

            if (!result.Success)
                return BadRequest(result.ErrorMessage);

            return Ok(new
            {
                success = true,
                msg = "Drug created successfully",
                drugId = result.DrugId,
                image = result.ImagePath
            });
        }


        // PUT: api/drugs/5
        [Authorize(Roles = "super_admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDrug(int id, [FromBody] DrugUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _drugService.UpdateDrugAsync(id, dto);

            if (!updated)
                return BadRequest(new { success = false, message = "Update failed" });

            return Ok(new { success = true, message = "Drug updated successfully" });
        }

        // DELETE: api/drugs/5
        [Authorize(Roles = "super_admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _drugService.DeleteDrugAsync(id);

            if (!deleted)
                return NotFound(new { message = "Drug not found" });

            return Ok(new { success = true, message = "Drug Deleted successfully" });
        }

        [Authorize(Roles = "super_admin")]
        [HttpGet("generate-barcode")]
        public async Task<IActionResult> GenerateBarcode()
        {
            var barcode = await _drugService.GenerateUniqueBarcodeAsync();
            return Ok(new { success = true, barcode });
        }

    }
    
}

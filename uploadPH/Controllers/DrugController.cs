using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pharmacy.Data; // Required for PharmacyDbContext
using Pharmacy.Dtos;
using Pharmacy.Models;
using Pharmacy.Repository;
using System.IO;

namespace Pharmacy.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DrugsController : ControllerBase
    {
        private readonly IDrugRepository _drugRepo;
        private readonly ITagRepository _tagRepo;
        private readonly PharmacyDbContext _context; // FIXED: Added missing field

        public DrugsController(IDrugRepository drugRepo, ITagRepository tagRepo, PharmacyDbContext context)
        {
            _drugRepo = drugRepo;
            _tagRepo = tagRepo;
            _context = context; // FIXED: Initialized field
        }

        // --- HELPER METHOD FOR CONSISTENCY ---
        private string? ValidateDrugLogic(string drugType, DateOnly? expirationDate)
        {
            var allowedTypes = new[]
            { "tablet", "syrup", "injection", "capsule", "cream", "gel", "spray", "drops" };

            if (!allowedTypes.Contains(drugType.ToLower()))
                return "Invalid drug type.";

            if (expirationDate.HasValue && expirationDate <= DateOnly.FromDateTime(DateTime.Today))
                return "Expiration date must be after today.";

            return null; // No errors
        }
        // -------------------------------------

        private DrugDto MapToDto(Drug d)
        {
            int totalAmount = d.ShelfAmount + d.StoredAmount;

            // Handle potential null Tags list if DTO is mapped from a query without Includes
            var tagNames = d.DrugTags != null 
                ? d.DrugTags.Select(dt => dt.Tag.Name).ToList() 
                : new List<string>();

            return new DrugDto
            {
                DrugId = d.DrugId,
                Name = d.Name,
                SellingPrice = d.SellingPrice,
                PurchasingPrice = d.PurchasingPrice,
                Barcode = d.Barcode,
                ImageUrl = d.ImageUrl,
                DescriptionBeforeUse = d.DescriptionBeforeUse,
                DescriptionHowToUse = d.DescriptionHowToUse,
                DescriptionSideEffects = d.DescriptionSideEffects,
                RequiresPrescription = d.RequiresPrescription,
                DrugType = d.DrugType,
                Manufacturer = d.Manufacturer,
                ExpirationDate = d.ExpirationDate,
                ShelfAmount = d.ShelfAmount,
                StoredAmount = d.StoredAmount,
                SubAmountQuantity = d.SubAmountQuantity,
                CreatedAt = d.CreatedAt,
                Tags = tagNames,
                IsLow = totalAmount <= d.LowAmount ? 1 : 0
            };
        }

        [HttpGet]
        public IActionResult GetAllDrugs()
        {
            var drugs = _drugRepo.GetAll();
            var dtoList = drugs.Select(MapToDto).ToList();

            return Ok(new
            {
                success = true,
                count = dtoList.Count,
                data = dtoList
            });
        }

        [HttpGet("{id}")]
        public IActionResult GetDrugById(int id)
        {
            var drug = _drugRepo.GetById(id);

            if (drug == null)
            {
                return NotFound(new
                {
                    success = false,
                    error = "Drug not found"
                });
            }

            var drugDto = MapToDto(drug);

            return Ok(new
            {
                success = true,
                data = drugDto
            });
        }

        [HttpDelete("{id}")]
       // [Authorize(Roles = "admin")]
        public IActionResult DeleteDrug(int id)
        {
            var drug = _drugRepo.GetById(id);

            if (drug == null)
            {
                return NotFound(new
                {
                    success = false,
                    error = $"Drug with ID {id} not found."
                });
            }

            _drugRepo.Delete(id);

            return Ok(new
            {
                success = true,
                message = "Drug deleted"
            });
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateDrug([FromForm] DrugCreateDto dto)
        {
            // 1. Consistent Validation Logic
            string? validationError = ValidateDrugLogic(dto.DrugType, dto.ExpirationDate);
            if (validationError != null)
                return BadRequest(validationError);

            // 2. Image Validation (Specific to Create)
            if (dto.Image == null || dto.Image.Length == 0)
                return BadRequest("Image is required.");

            // 3. Save Image
            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsPath))
                Directory.CreateDirectory(uploadsPath);

            var fileName = $"{Guid.NewGuid()}_{dto.Image.FileName}";
            var filePath = Path.Combine(uploadsPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.Image.CopyToAsync(stream);
            }

            // 4. Map DTO to Entity
            var drug = new Drug
            {
                Name = dto.Name,
                SellingPrice = dto.SellingPrice,
                PurchasingPrice = dto.PurchasingPrice,
                Barcode = dto.Barcode,
                ImageUrl = "/uploads/" + fileName,
                DescriptionBeforeUse = dto.DescriptionBeforeUse,
                DescriptionHowToUse = dto.DescriptionHowToUse,
                DescriptionSideEffects = dto.DescriptionSideEffects,
                RequiresPrescription = dto.RequiresPrescription,
                DrugType = dto.DrugType,
                Manufacturer = dto.Manufacturer,
                ExpirationDate = dto.ExpirationDate,
                ShelfAmount = dto.ShelfAmount,
                StoredAmount = dto.StoredAmount,
                LowAmount = dto.LowAmount,
                SubAmountQuantity = dto.SubAmountQuantity,
                CreatedAt = DateTime.Now,
                DrugTags = new List<DrugTag>()
            };

            // 5. Handle Tags
            if (dto.Tags != null && dto.Tags.Any())
            {
                var tagNames = dto.Tags.Distinct().Select(n => n.ToLower()).ToList();
                List<Tag> existingTags = await _tagRepo.GetTagsByNamesAsync(tagNames);

                var existingTagNames = existingTags.Select(t => t.Name.ToLower()).ToList();
                var newTagNames = tagNames.Except(existingTagNames, StringComparer.OrdinalIgnoreCase).ToList();
                var newTags = newTagNames.Select(name => new Tag { Name = name }).ToList();

                _tagRepo.AddRange(newTags);
                await _tagRepo.SaveAsync(); // Save TAGS (Async allowed here)

                var allTags = existingTags.Concat(newTags);

                foreach (var tag in allTags)
                {
                    drug.DrugTags.Add(new DrugTag { TagId = tag.TagId });
                }
            }

            // 6. Save Drug
            _drugRepo.Add(drug);
            _drugRepo.Save(); // FIXED: Removed await (Sync method)

            return Ok(new { message = "Drug created successfully", drugId = drug.DrugId });
        }

        [HttpPut("{id}")]
       // [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateDrug(int id, [FromBody] DrugUpdateDto dto)
        {
            // 1. Consistent Validation Logic
            string? validationError = ValidateDrugLogic(dto.DrugType, dto.ExpirationDate);
            if (validationError != null)
                return BadRequest(validationError);

            // 2. Fetch existing drug (No Tracking)
            var existingDrug = await _context.Drugs
                .AsNoTracking()
                .Where(d => d.DrugId == id)
                .Include(d => d.DrugTags)
                .ThenInclude(dt => dt.Tag)
                .FirstOrDefaultAsync();

            if (existingDrug == null)
            {
                return NotFound(new { success = false, error = $"Drug with ID {id} not found." });
            }

            // 3. Map DTO to Entity (Full Replacement)
            var updatedDrug = new Drug
            {
                DrugId = id,
                Name = dto.Name,
                SellingPrice = dto.SellingPrice,
                PurchasingPrice = dto.PurchasingPrice,
                Barcode = dto.Barcode,
                DrugType = dto.DrugType,
                ExpirationDate = dto.ExpirationDate,
                ShelfAmount = dto.ShelfAmount,
                StoredAmount = dto.StoredAmount,
                SubAmountQuantity = dto.SubAmountQuantity,
                LowAmount = dto.LowAmount,

                // Preserve non-updated fields
                ImageUrl = existingDrug.ImageUrl,
                CreatedAt = existingDrug.CreatedAt,
                DescriptionBeforeUse = existingDrug.DescriptionBeforeUse,
                DescriptionHowToUse = existingDrug.DescriptionHowToUse,
                DescriptionSideEffects = existingDrug.DescriptionSideEffects,
                RequiresPrescription = existingDrug.RequiresPrescription,
                Manufacturer = existingDrug.Manufacturer,

                DrugTags = new List<DrugTag>()
            };

            // 4. Handle Tags
            if (dto.Tags != null && dto.Tags.Any())
            {
                var tagNames = dto.Tags.Distinct().Select(n => n.ToLower()).ToList();
                List<Tag> existingTags = await _tagRepo.GetTagsByNamesAsync(tagNames);

                var existingTagNames = existingTags.Select(t => t.Name.ToLower()).ToList();
                var newTagNames = tagNames.Except(existingTagNames, StringComparer.OrdinalIgnoreCase).ToList();
                var newTags = newTagNames.Select(name => new Tag { Name = name }).ToList();

                _tagRepo.AddRange(newTags);
                await _tagRepo.SaveAsync(); // Save TAGS (Async allowed here)

                var allTags = existingTags.Concat(newTags);

                foreach (var tag in allTags)
                {
                    updatedDrug.DrugTags.Add(new DrugTag
                    {
                        TagId = tag.TagId,
                        DrugId = id
                    });
                }
            }

            // 5. Save Updates
            _drugRepo.Update(updatedDrug); // FIXED: No await needed here if Update is void

            return Ok(new
            {
                success = true,
                message = "Drug updated successfully"
            });
        }
    }
}
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
        private readonly PharmacyDbContext _context; 

        public DrugsController(IDrugRepository drugRepo, ITagRepository tagRepo, PharmacyDbContext context)
        {
            _drugRepo = drugRepo;
            _tagRepo = tagRepo;
            _context = context; 
        }

        private string? ValidateDrugLogic(string drugType, DateOnly? expirationDate)
        {
            var allowedTypes = new[]
            { "tablet", "syrup", "injection", "capsule", "cream", "gel", "spray", "drops" };

            if (!allowedTypes.Contains(drugType.ToLower()))
                return "Invalid drug type.";

            if (expirationDate.HasValue && expirationDate <= DateOnly.FromDateTime(DateTime.Today))
                return "Expiration date must be after today.";

            return null; 
        }

        private DrugDto MapToDto(Drug d)
        {
            int totalAmount = d.ShelfAmount + d.StoredAmount;


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
        [Authorize(Roles = "super_admin")]
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
        [Authorize(Roles = "super_admin")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateDrug([FromForm] DrugCreateDto dto)
        {
            string? validationError = ValidateDrugLogic(dto.DrugType, dto.ExpirationDate);
            if (validationError != null)
                return BadRequest(validationError);

            string? fileName = null;

            // Handle image only if provided
            if (dto.Image != null && dto.Image.Length > 0)
            {
                var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsPath))
                    Directory.CreateDirectory(uploadsPath);

                fileName = $"{Guid.NewGuid()}_{dto.Image.FileName}";
                var filePath = Path.Combine(uploadsPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.Image.CopyToAsync(stream);
                }
            }

            var drug = new Drug
            {
                Name = dto.Name,
                SellingPrice = dto.SellingPrice,
                PurchasingPrice = dto.PurchasingPrice,
                Barcode = dto.Barcode,

                // ❗ If no image uploaded, ImageUrl becomes null
                ImageUrl = fileName != null ? "/uploads/" + fileName : null,

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

            // --------- TAG LOGIC (unchanged) ----------
            if (dto.Tags != null && dto.Tags.Any())
            {
                var tagNames = dto.Tags.Distinct().Select(n => n.ToLower()).ToList();
                List<Tag> existingTags = await _tagRepo.GetTagsByNamesAsync(tagNames);

                var existingTagNames = existingTags.Select(t => t.Name.ToLower()).ToList();
                var newTagNames = tagNames.Except(existingTagNames, StringComparer.OrdinalIgnoreCase).ToList();
                var newTags = newTagNames.Select(name => new Tag { Name = name }).ToList();

                _tagRepo.AddRange(newTags);
                await _tagRepo.SaveAsync();

                var allTags = existingTags.Concat(newTags);

                foreach (var tag in allTags)
                {
                    drug.DrugTags.Add(new DrugTag { TagId = tag.TagId });
                }
            }

            _drugRepo.Add(drug);
            _drugRepo.Save();

            return Ok(new { message = "Drug created successfully", drugId = drug.DrugId });
        }


        [HttpPut("{id}")]
        [Authorize(Roles = "super_admin")]
        public async Task<IActionResult> UpdateDrug(int id, [FromBody] DrugUpdateDto dto)
        {
            string? validationError = ValidateDrugLogic(dto.DrugType, dto.ExpirationDate);
            if (validationError != null)
                return BadRequest(validationError);

            var existingDrug = await _context.Drugs
                .AsNoTracking()
                .Include(d => d.DrugTags)
                .FirstOrDefaultAsync(d => d.DrugId == id);

            if (existingDrug == null)
                return NotFound(new { success = false, error = $"Drug with ID {id} not found." });

            // Prevent duplicate barcode
            var duplicateBarcode = await _context.Drugs
                .AnyAsync(d => d.Barcode == dto.Barcode && d.DrugId != id);

            if (duplicateBarcode)
                return Conflict("A drug with this barcode already exists.");

            var updatedDrug = new Drug
            {
                DrugId = id,
                Name = dto.Name ?? existingDrug.Name,
                SellingPrice = dto.SellingPrice,
                PurchasingPrice = dto.PurchasingPrice,
                Barcode = dto.Barcode ?? existingDrug.Barcode,
                DrugType = dto.DrugType,
                ExpirationDate = dto.ExpirationDate,
                ShelfAmount = dto.ShelfAmount,
                StoredAmount = dto.StoredAmount,
                SubAmountQuantity = dto.SubAmountQuantity,
                LowAmount = dto.LowAmount,

                ImageUrl = existingDrug.ImageUrl,
                CreatedAt = existingDrug.CreatedAt,
                DescriptionBeforeUse = existingDrug.DescriptionBeforeUse,
                DescriptionHowToUse = existingDrug.DescriptionHowToUse,
                DescriptionSideEffects = existingDrug.DescriptionSideEffects,
                RequiresPrescription = existingDrug.RequiresPrescription,
                Manufacturer = existingDrug.Manufacturer,

                DrugTags = new List<DrugTag>()
            };

            if (dto.Tags != null && dto.Tags.Any())
            {
                var tagNames = dto.Tags
                    .Distinct()
                    .Select(n => n.ToLower())
                    .ToList();

                var existingTags = await _tagRepo.GetTagsByNamesAsync(tagNames);

                var newTagNames = tagNames
                    .Except(existingTags.Select(t => t.Name.ToLower()))
                    .ToList();

                var newTags = newTagNames.Select(name => new Tag { Name = name }).ToList();
                _tagRepo.AddRange(newTags);
                await _tagRepo.SaveAsync();

                var allTags = existingTags.Concat(newTags);

                foreach (var tag in allTags)
                    updatedDrug.DrugTags.Add(new DrugTag { TagId = tag.TagId, DrugId = id });
            }
            else
            {
                updatedDrug.DrugTags = existingDrug.DrugTags; // keep old tags
            }

            _drugRepo.Update(updatedDrug);
            _drugRepo.Save();

            return Ok(new { success = true, message = "Drug updated successfully" });
        }
    }
}
using AutoMapper;
using Pharmacy.Dtos;
using Pharmacy.Models;
using Pharmacy.Models.Dto;
using Pharmacy.Repository;

namespace Pharmacy.Services
{
    public class DrugService : IDrugService
    {
        private readonly IDrugRepository _repo;
        private readonly IMapper _mapper;

        public DrugService(IDrugRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<List<DrugsummaryDto>> GetAllDrugsAsync()
        {
            var drugs = await _repo.GetAllAsync();
            return _mapper.Map<List<DrugsummaryDto>>(drugs);
        }

        public async Task<DrugsummaryDto?> GetDrugByIdAsync(int id)
        {
            var drug = await _repo.GetByIdAsync(id);
            return drug == null ? null : _mapper.Map<DrugsummaryDto>(drug);
        }

        public async Task<DrugCreated> CreateDrugAsync(DrugCreateDto dto, string imageUrl)
        {
       
            if (dto.SellingPrice < dto.PurchasingPrice)
            {
                return new DrugCreated
                {
                    Success = false,
                    ErrorMessage = "Selling price cannot be less than purchasing price"
                };
            }

            if (dto.ExpirationDate.HasValue &&
                dto.ExpirationDate <= DateOnly.FromDateTime(DateTime.Now))
            {
                return new DrugCreated
                {
                    Success = false,
                    ErrorMessage = "Expiration date must be in the future"
                };
            }

            var drug = _mapper.Map<Drug>(dto);
            drug.ImageUrl = imageUrl;

            if (dto.Tags != null)
            {
                foreach (var tagName in dto.Tags.Where(t => !string.IsNullOrWhiteSpace(t)))
                {
                    var tag = await _repo.GetTagByNameAsync(tagName);

                    if (tag == null)
                        tag = await _repo.CreateTagAsync(tagName);

                    drug.DrugTags.Add(new DrugTag
                    {
                        TagId = tag.TagId,
                        Drug = drug
                    });
                }
            }

            await _repo.AddAsync(drug);

            return new DrugCreated
            {
                Success = true,
                DrugId = drug.DrugId,
                ImagePath = drug.ImageUrl
            };
        }

        public async Task<bool> UpdateDrugAsync(int id, DrugUpdateDto dto)
        {
            var existing = await _repo.GetByIdWithTagsAsync(id);
            if (existing == null) return false;

            var allowedTypes = new List<string>
            {
                "drops", "spray", "gel", "cream",
                "capsule", "injection", "syrup", "tablet"
            };

            if (!allowedTypes.Contains(dto.DrugType?.ToLower()))
                return false;

            _mapper.Map(dto, existing);

          
            if (dto.Tags != null)
            {
                existing.DrugTags.Clear();

                foreach (var name in dto.Tags)
                {
                    var tag = await _repo.GetTagByNameAsync(name);
                    if (tag == null) return false;

                    existing.DrugTags.Add(new DrugTag
                    {
                        DrugId = existing.DrugId,
                        TagId = tag.TagId
                    });
                }
            }

            await _repo.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteDrugAsync(int id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return false;

            await _repo.DeleteAsync(id);
            return true;
        }

        public async Task<List<DrugsummaryDto>> SearchDrugsAsync(string? name, string? barcode, string? type, string? tags)
        {
            List<string>? tagList = null;
            if (!string.IsNullOrEmpty(tags))
            {
                tagList = tags.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                              .ToList();
            }

            var drugsQuery = await _repo.SearchAsync(name, barcode, type, tagList);
            return _mapper.Map<List<DrugsummaryDto>>(drugsQuery);
        }
        public async Task<string> GenerateUniqueBarcodeAsync()
        {
            string barcode;
            bool exists;

            do
            {
                barcode = GenerateRandomBarcode();
                exists = await _repo.BarcodeExistsAsync(barcode);
            } while (exists);

            return barcode;
        }

       
        private string GenerateRandomBarcode()
        {
            var random = new Random();
            return string.Concat(Enumerable.Range(0, 12).Select(_ => random.Next(0, 10).ToString()));
        }
    }
}

using AutoMapper;
using Pharmacy.Models;
using Pharmacy.Models.Dto;
using Pharmacy.Repository;

namespace Pharmacy.Services
{
    public class InvoiceService:IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepo;
        private readonly IDrugRepository _drugRepo;
        private readonly IInvoiceItemRepository _itemRepo;
        private readonly IMapper _mapper;

        public InvoiceService(
            IInvoiceRepository invoiceRepo,
            IDrugRepository drugRepo,
            IInvoiceItemRepository itemRepo,
            IMapper mapper)
        {
            _invoiceRepo = invoiceRepo;
            _drugRepo = drugRepo;
            _itemRepo = itemRepo;
            _mapper = mapper;
        }
        public async Task<List<InvoiceSummaryDto>> GetAllInvoicesAsync()
        {
            var invoices = await _invoiceRepo.GetAllAsync();
            return _mapper.Map<List<InvoiceSummaryDto>>(invoices);
        }
        public async Task<InvoiceDetailsDto> GetInvoiceByIdAsync(int invoiceId)
        {
            var invoice = await _invoiceRepo.GetByIdAsync(invoiceId);

            if (invoice == null)
                throw new KeyNotFoundException("Invoice not found");

            return _mapper.Map<InvoiceDetailsDto>(invoice);
        }
        public async Task<List<InvoiceDateDto>> GetInvoicesByDateRangeAsync( DateTime from, DateTime to)
        {
            if (from > to)
                throw new ArgumentException("'from' date cannot be after 'to' date");

            var invoices = await _invoiceRepo.GetByDateRangeAsync(from, to);
            return _mapper.Map<List<InvoiceDateDto>>(invoices);
        }
        public async Task<InvoiceResponseDto> CreateInvoiceAsync(CreateInvoiceDto dto)
        {
            var invoice = new Invoice
            {
                AdminId = dto.AdminId,
                DiscountAmount = dto.DiscountAmount,
                TaxAmount = dto.TaxAmount,
                ChangeAmount = dto.ChangeAmount,
                InvoiceTime = DateTime.UtcNow
            };

            decimal totalAmount = 0;
            var items = new List<InvoiceItem>();

          
            foreach (var itemDto in dto.Items)
            {
                var drug = await _drugRepo.GetByIdAsync(itemDto.DrugId);
                if (drug == null)
                    throw new KeyNotFoundException($"Drug {itemDto.DrugId} not found");

                var item = new InvoiceItem
                {
                    Invoice = invoice,
                    Drug = drug,
                    DrugId = itemDto.DrugId,
                    Quantity = itemDto.Quantity
                };

                totalAmount += drug.PurchasingPrice * itemDto.Quantity;
                items.Add(item);
            }
            invoice.TotalAmount = totalAmount - dto.DiscountAmount + dto.TaxAmount;

            
            await _invoiceRepo.AddAsync(invoice);
            await _invoiceRepo.SaveChangesAsync();

          
            foreach (var item in items)
            {
                await _itemRepo.AddAsync(item);
            }
            await _itemRepo.SaveChangesAsync();
            invoice.InvoiceItems = items;

            
            return _mapper.Map<InvoiceResponseDto>(invoice);
        }


        public async Task<InvoiceItemResponseDto> AddItemToInvoiceAsync(
       int invoiceId, InvoiceItemDto dto)
        {
            var invoice = await _invoiceRepo.GetByIdAsync(invoiceId);
            if (invoice == null)
                throw new KeyNotFoundException("Invoice not found");

            var drug = await _drugRepo.GetByIdAsync(dto.DrugId);
            if (drug == null)
                throw new KeyNotFoundException("Drug not found");

            var item = new InvoiceItem
            {
                InvoiceId = invoiceId,
                DrugId = dto.DrugId,
                Quantity = dto.Quantity,
                Drug = drug 
            };

            await _itemRepo.AddAsync(item);
            await _itemRepo.SaveChangesAsync();

            return _mapper.Map<InvoiceItemResponseDto>(item);
        }
        public async Task DeleteInvoiceAsync(int invoiceId)
        {
            var invoice = await _invoiceRepo.GetByIdForDeleteAsync(invoiceId);

            if (invoice == null)
                throw new KeyNotFoundException("Invoice not found");

            await _invoiceRepo.DeleteAsync(invoice);
            await _invoiceRepo.SaveChangesAsync();
        }
        public async Task<List<InvoiceItemDetailsDto>> GetItemsByInvoiceIdAsync(int invoiceId)
        {
           
            var invoiceExists = await _itemRepo.ExistsAsync(invoiceId);
            if (!invoiceExists)
                throw new KeyNotFoundException("Invoice not found");

            var items = await _itemRepo.GetItemsByInvoiceIdAsync(invoiceId);
            return _mapper.Map<List<InvoiceItemDetailsDto>>(items);
        }
        public async Task RemoveInvoiceItemAsync(int itemId, int quantityToRemove = 1)
        {
            var item = await _itemRepo.GetByIdAsync(itemId);
            if (item == null)
                throw new KeyNotFoundException("Invoice item not found");

            var invoice = await _invoiceRepo.GetByIdWithItemsAsync(item.InvoiceId);
            if (invoice == null)
                throw new KeyNotFoundException("Invoice not found");

            
            if (quantityToRemove >= item.Quantity)
            {
               
                await _itemRepo.DeleteAsync(item);
            }
            else
            {
               
                item.Quantity -= quantityToRemove;
            }

            await _itemRepo.SaveChangesAsync();

           
            decimal itemsTotal = invoice.InvoiceItems
                .Where(ii => ii.ItemId != itemId || ii.Quantity > 0)
                .Sum(ii => ii.Drug.PurchasingPrice * ii.Quantity);

            invoice.TotalAmount = itemsTotal - invoice.DiscountAmount + invoice.TaxAmount;

            await _invoiceRepo.SaveChangesAsync();
        }



    }
}



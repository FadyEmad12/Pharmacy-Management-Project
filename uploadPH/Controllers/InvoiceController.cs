using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pharmacy.Models.Dto;
using Pharmacy.Services;

namespace Pharmacy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
     private readonly IInvoiceService _service;

        public InvoiceController(IInvoiceService service)
        {
            _service = service;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllInvoices()
        {
            var invoices = await _service.GetAllInvoicesAsync();
            return Ok(invoices);
        }
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetInvoiceById(int id)
        {
            try
            {
                var invoice = await _service.GetInvoiceByIdAsync(id);
                return Ok(invoice);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
        [HttpGet("range")]
        public async Task<IActionResult> GetInvoicesByDateRange([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            try
            {
                var invoices = await _service.GetInvoicesByDateRangeAsync(from, to);
                return Ok(invoices);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
        [HttpGet("{invoiceId:int}/items")]
        public async Task<IActionResult> GetInvoiceItems(int invoiceId)
        {
            try
            {
                var items = await _service.GetItemsByInvoiceIdAsync(invoiceId);
                return Ok(items);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }


        [HttpPost]
        public async Task<IActionResult> CreateInvoice([FromBody] CreateInvoiceDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var invoice = await _service.CreateInvoiceAsync(dto);

            var response = new ApiResponse<InvoiceResponseDto>
            {
                Success = true,
                Invoice = invoice
            };

            return CreatedAtAction(nameof(CreateInvoice), response);
        }
        [HttpPost("{invoiceId:int}/items")]
        public async Task<IActionResult> AddItemToInvoice(int invoiceId, [FromBody] InvoiceItemDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
              
                var item = await _service.AddItemToInvoiceAsync(invoiceId, dto);

                
                return Created("", new
                {
                    success = true,
                    item = new
                    {
                        item_id = item.ItemId,
                        drug_id = item.DrugId,
                        quantity = item.Quantity,
                        price_per_unit = item.PricePerUnit,
                        total_price = item.TotalPrice
                    }
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteInvoice(int id)
        {
            try
            {
                await _service.DeleteInvoiceAsync(id);

                return Ok(new
                {
                    success = true,
                    message = "Invoice deleted."
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
        
        [HttpDelete("items/{itemId:int}")]
        public async Task<IActionResult> RemoveItem(int itemId, [FromQuery] int quantity = 1)
        {
            try
            {
                await _service.RemoveInvoiceItemAsync(itemId, quantity);
                return Ok(new { success = true });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
        }
       

    }
}


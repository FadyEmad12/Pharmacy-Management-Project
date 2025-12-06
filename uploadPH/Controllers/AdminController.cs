// In Pharmacy.Controllers/AdminController.cs

using Microsoft.AspNetCore.Mvc;
using Pharmacy.Dtos;
using Pharmacy.Models;
using Pharmacy.Repository;
using Pharmacy.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Pharmacy.Data;

namespace Pharmacy.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminRepository _adminRepo;
        private readonly IPasswordHelper _passwordHelper;
        private readonly PharmacyDbContext _context; // <--- 1. ADD THIS FIELD

        public AdminController(IAdminRepository adminRepo, IPasswordHelper passwordHelper, PharmacyDbContext context) // <--- 2. ADD 'PharmacyDbContext context'
        {
            _adminRepo = adminRepo;
            _passwordHelper = passwordHelper;
            _context = context; // <--- 3. INITIALIZE IT HERE
        }

        private AdminResponseDto MapToDto(Admin admin)
        {
            return new AdminResponseDto
            {
                AdminId = admin.AdminId,
                Username = admin.Username,
                Email = admin.Email,
                Role = admin.Role,
                CreatedAt = admin.CreatedAt
            };
        }
        
        // --- PUBLIC ENDPOINTS (AUTHENTICATION) ---

        [HttpPost("signup")]
        public IActionResult SignUp([FromBody] AdminSignupDto dto)
        {
            // 1. Validate role consistency
            if (dto.Role.ToLower() != "super_admin" && dto.Role.ToLower() != "cashier")
            {
                return BadRequest(new { success = false, error = "Invalid role specified." });
            }

            // 2. Check if user already exists
            if (_adminRepo.GetByEmail(dto.Email) != null)
            {
                return Conflict(new { success = false, error = "Email is already registered." });
            }

            // 3. Hash Password
            string passwordHash = _passwordHelper.HashPassword(dto.Password);

            // 4. Create new Admin
            var newAdmin = new Admin
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = passwordHash,
                Role = dto.Role.ToLower(),
                CreatedAt = DateTime.UtcNow
            };

            _adminRepo.Add(newAdmin);
            _adminRepo.Save();

            // 5. Successful response (We'll handle cookie/token creation separately for now)
            // Note: In a real system, you would generate a JWT token or set an authentication cookie here.
            
            return Ok(new
            {
                success = true,
                message = "User created successfully. Log in to get a cookie/token."
            });
        }
        
        // You will need a 'login' endpoint here that verifies password and sets the cookie.

        // --- AUTHENTICATED/AUTHORIZED ENDPOINTS (USER MANAGEMENT) ---

        [HttpGet]
        [Authorize(Roles = "super_admin")]
        public IActionResult GetAllSystemUsers()
        {
            var admins = _adminRepo.GetAll();
            var dtoList = admins.Select(MapToDto).ToList();

            return Ok(new
            {
                success = true,
                count = dtoList.Count,
                data = dtoList
            });
        }
        
        // The POST endpoint is already handled by the 'signup' logic above.

        [HttpPut("{id}")]
        [Authorize(Roles = "super_admin")]
        public IActionResult UpdateSystemUser(int id, [FromBody] AdminSignupDto dto)
        {
            var existingAdmin = _adminRepo.GetByEmail(dto.Email);
            
            // 1. Check if the email is being used by another admin
            if (existingAdmin != null && existingAdmin.AdminId != id)
            {
                return Conflict(new { success = false, error = "Email is already in use by another user." });
            }
            
            // 2. Fetch the entity to update
            var adminToUpdate = _context.Admins.Find(id);

            if (adminToUpdate == null)
            {
                return NotFound(new { success = false, error = "Admin not found." });
            }
            
            // 3. Update fields
            adminToUpdate.Username = dto.Username;
            adminToUpdate.Email = dto.Email;
            adminToUpdate.Role = dto.Role.ToLower();
            
            // Hash the password only if it was provided (the PUT request serves as a full update, 
            // but we usually require password hashing)
            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                adminToUpdate.PasswordHash = _passwordHelper.HashPassword(dto.Password);
            }

            _adminRepo.Update(adminToUpdate);
            _adminRepo.Save();

            return Ok(new
            {
                success = true,
                message = "Admin updated successfully"
            });
        }
        
        [HttpDelete("{id}")]
        [Authorize(Roles = "super_admin")]
        public IActionResult DeleteSystemUser(int id)
        {
            var adminToDelete = _adminRepo.GetAll().FirstOrDefault(a => a.AdminId == id);
            
            if (adminToDelete == null)
            {
                // Return success if the user is already gone (idempotency)
                return Ok(new { success = true, message = "Admin deleted" });
            }
            
            _adminRepo.Delete(id);
            _adminRepo.Save();

            return Ok(new { success = true, message = "Admin deleted" });
        }
    }
}
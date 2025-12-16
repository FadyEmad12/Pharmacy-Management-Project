using Microsoft.AspNetCore.Mvc;
using Pharmacy.Dtos;
using Pharmacy.Models;
using Pharmacy.Repository;
using Pharmacy.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Pharmacy.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;


namespace Pharmacy.Controllers
{
   
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminRepository _adminRepo;
        private readonly IPasswordHelper _passwordHelper;
        private readonly PharmacyDbContext _context; 
        private readonly IAdminLogService _logService;

        public AdminController(IAdminRepository adminRepo, IPasswordHelper passwordHelper, PharmacyDbContext context , IAdminLogService logService) 
        {
            _adminRepo = adminRepo;
            _passwordHelper = passwordHelper;
            _context = context; 
            _logService = logService;
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
        
        [Authorize(Roles = "super_admin")]
        [HttpPost("signup")]
        public IActionResult SignUp([FromBody] AdminSignupDto dto)
        {
            if (dto.Role.ToLower() != "super_admin" && dto.Role.ToLower() != "cashier")
            {
                return BadRequest(new { success = false, error = "Invalid role specified." });
            }

            if (_adminRepo.GetByEmail(dto.Email) != null)
            {
                return Conflict(new { success = false, error = "Email is already registered." });
            }

            string passwordHash = _passwordHelper.HashPassword(dto.Password);

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

            return Ok(new
            {
                success = true,
                message = "User created successfully."
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(AdminLoginDto dto)
        {
            var admin = _adminRepo.GetAll()
                .FirstOrDefault(a => a.Username == dto.Username);

            if (admin == null)
                return Unauthorized(new { success = false, error = "Invalid credentials." });

            bool isPasswordCorrect = _passwordHelper.VerifyPassword(dto.Password, admin.PasswordHash);
            if (!isPasswordCorrect)
                return Unauthorized(new { success = false, error = "Invalid credentials." });

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, admin.AdminId.ToString()),
                new Claim(ClaimTypes.Name, admin.Username),
                new Claim(ClaimTypes.Role, admin.Role)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddDays(7)
                }
            );

            // Log the login action
            await _logService.LogAsync(admin.AdminId, "login");

            return Ok(new
            {
                success = true,
                message = "Logged in successfully.",
                user = new
                {
                    username = admin.Username,
                    role = admin.Role
                }
            });
        }

        [Authorize(Roles = "super_admin")]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var adminIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(adminIdValue) || !int.TryParse(adminIdValue, out int adminId))
                return Unauthorized("Invalid or missing admin identity.");

            await HttpContext.SignOutAsync();

            await _logService.LogAsync(adminId, "logout");

            return Ok(new { success = true, message = "Logged out successfully." });
        }



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
        
        [HttpPut("{id}")]
        [Authorize(Roles = "super_admin")]
        public IActionResult UpdateSystemUser(int id, [FromBody] AdminSignupDto dto)
        {
            var existingAdmin = _adminRepo.GetByEmail(dto.Email);
            
            if (existingAdmin != null && existingAdmin.AdminId != id)
            {
                return Conflict(new { success = false, error = "Email is already in use by another user." });
            }
            
            var adminToUpdate = _context.Admins.Find(id);

            if (adminToUpdate == null)
            {
                return NotFound(new { success = false, error = "Admin not found." });
            }
            
            adminToUpdate.Username = dto.Username;
            adminToUpdate.Email = dto.Email;
            adminToUpdate.Role = dto.Role.ToLower();
            
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
                return Ok(new { success = true, message = "Admin deleted" });
            }
            
            _adminRepo.Delete(id);
            _adminRepo.Save();

            return Ok(new { success = true, message = "Admin deleted" });
        }


        [HttpGet("logs")]
        [Authorize(Roles = "super_admin")]
        public async Task<IActionResult> GetAdminLogs([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            if (page < 1 || pageSize < 1)
                return BadRequest(new { success = false, error = "page and pageSize must be positive integers." });

            var query = _context.AdminLogs
                .Include(l => l.Admin)
                .OrderByDescending(l => l.ActionTime)
                .AsQueryable();

            int totalRecords = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            var logs = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(log => new
                {
                    logId = log.LogId,
                    adminId = log.AdminId,
                    adminUsername = log.Admin.Username,
                    actionType = log.ActionType,
                    actionTime = log.ActionTime
                })
                .ToListAsync();

            return Ok(new
            {
                success = true,
                page,
                pageSize,
                totalRecords,
                totalPages,
                records = logs
            });
        }
    }
}
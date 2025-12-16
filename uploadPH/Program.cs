using Microsoft.EntityFrameworkCore;
using Pharmacy.Models;
using Pharmacy.Repository;
using Pharmacy.Services;
using Pharmacy.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using AutoMapper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/api/admin/login";
        options.AccessDeniedPath = "/api/admin/access-denied";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.SecurePolicy = CookieSecurePolicy.None; // change to Always in production (HTTPS)
        options.ExpireTimeSpan = TimeSpan.FromDays(7); // session length
    });

builder.Services.AddAuthorization();

// Register DbContext with SQL Server
builder.Services.AddDbContext<PharmacyDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))); // i have  used a new connection string that will work with any sql server express so you dont need to change it later

// Register Repository
builder.Services.AddScoped<IDrugRepository, DrugRepository>();
builder.Services.AddScoped<IDrugService, DrugService>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IPasswordHelper, PasswordHelper>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>(); 
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IInvoiceItemRepository, InvoiceItemRepository>();
builder.Services.AddScoped<DatabaseSeeder>();
builder.Services.AddScoped<IAdminLogService, AdminLogService>();


// Add Swagger for API testing
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(Program));

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//sedding the database

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
    seeder.Seed();
}



app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseStaticFiles(); // for the images to server them 

app.MapControllers();

app.Run();




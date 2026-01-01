using Microsoft.EntityFrameworkCore;
using Pharmacy.Repository;
using Pharmacy.Services;
using Pharmacy.Data;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// --- 1. SERVICES ---
builder.Services.AddControllers();

// 1. JWT Configuration
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["Key"] ;
var key = Encoding.UTF8.GetBytes(secretKey);

// ONLY JWT Authentication (Removed the old AddCookie code to avoid conflicts)
builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddAuthorization();

// 2. Relaxed CORS for your team (Allowing everything for easy HTML testing)
builder.Services.AddCors(options => {
    options.AddPolicy("AllowFrontend", policy => {
        policy.AllowAnyOrigin() 
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<PharmacyDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Dependency Injection
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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Pharmacy API", Version = "v1" });

    // 1. Define the Security Scheme
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\""
    });

    // 2. Make Swagger use that scheme globally
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
builder.Services.AddAutoMapper(typeof(Program));

var app = builder.Build();

// --- 2. STARTUP LOGIC ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var seeder = services.GetRequiredService<DatabaseSeeder>();
        seeder.Seed();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Startup Error: {ex.Message}");
    }
}

// --- 3. MIDDLEWARE ORDER (CRITICAL) ---
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting(); 

// Apply CORS Policy here
app.UseCors("AllowFrontend"); 

// Use Authentication BEFORE Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
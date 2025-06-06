using BHDTest.DTOs;
using BHDTest.Models;
using BHDTest.Services;
using BHDTest.Validators;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Entity Framework
builder.Services.AddDbContext<BHDPruebaContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("BHDTestConnection"));
});

// Validators
builder.Services.AddTransient<IValidator<UserCreateRequestDto>, UserCreateRequestValidator>();

//JWT Configuration
string key = builder.Configuration["Jwt:Key"]!;
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
builder.Services.AddAuthorization();
builder.Services.AddAuthentication("Bearer").AddJwtBearer(opt =>
{
    var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256Signature);

    opt.RequireHttpsMetadata = false;
    opt.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateAudience = false,
        ValidateIssuer = false,
        IssuerSigningKey = signingKey,
    };
});

//Services Dependency  injection 
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

//Map Route
app.MapGet("/", () => "Prueba");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

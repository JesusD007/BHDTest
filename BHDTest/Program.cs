using BHDTest.DTOs;
using BHDTest.Models;
using BHDTest.Validators;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Entity Framework
builder.Services.AddDbContext<BHDPruebaContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("BHDTestConnection"));
});

// Validators
builder.Services.AddTransient<IValidator<UserCreateRequestDto>, UserCreateRequestValidator>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

using BHDTest.DTOs;
using FluentValidation;
using System.Data;

namespace BHDTest.Validators
{

    public class UserCreateRequestValidator : AbstractValidator<UserCreateRequestDto>
    {
        private readonly IConfiguration _configuration;
        public UserCreateRequestValidator(IConfiguration configuration) {
            
            // Dependency injection
            _configuration = configuration;

            // Validations
            // name
            RuleFor(x=>x.Nombre).NotEmpty().WithMessage("El nombre es obligatorio.");

            // email;
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El email es obligatorio.")
                .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$").WithMessage("El formato del email no es válido.");

            // pass
            RuleFor(x => x.Password)
              .NotEmpty()
              .Matches(_configuration["PasswordRules:Regex"]).WithMessage("La contraseña debe tener al menos 8 caracteres, una mayúscula, una minúscula, un número y un carácter especial.");
        }
    }
}

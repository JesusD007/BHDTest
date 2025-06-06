using BHDTest.Models;

namespace BHDTest.DTOs
{
    public class UserCreateRequestDto
    {
        public string? Nombre { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public List<PhoneRequestDto>? Phones { get; set; }
    }
}

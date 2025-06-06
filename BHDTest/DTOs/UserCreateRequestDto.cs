using BHDTest.Models;

namespace BHDTest.DTOs
{
    public class UserRequestDto
    {
            public string? Nombre { get; set; }
            public string? Email { get; set; }
            public string? Password { get; set; }
            public List<PhoneRequestDto>? Phones { get; set; }
    }
}

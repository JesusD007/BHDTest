using BHDTest.Models;
using System.ComponentModel.DataAnnotations;

namespace BHDTest.DTOs
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public List<PhoneDto> Phones { get; set; } = new List<PhoneDto>();
    }
}

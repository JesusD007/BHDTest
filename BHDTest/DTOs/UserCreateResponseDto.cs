using BHDTest.Models;
using System.ComponentModel.DataAnnotations;

namespace BHDTest.DTOs
{
    public class UserResponseDto
    {
        public Guid Id { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        public DateTime LastLogin { get; set; }

        public string Token { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}

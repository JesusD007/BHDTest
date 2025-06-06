using BHDTest.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BHDTest.DTOs
{
    public class PhoneDto
    {
        public int Id { get; set; }
        public string Number { get; set; } = string.Empty;
        public string CityCode { get; set; } = string.Empty;
        public string CountryCode { get; set; } = string.Empty;
        public User? User { get; set; }
    }
}

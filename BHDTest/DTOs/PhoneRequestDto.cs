using System.ComponentModel.DataAnnotations;

namespace BHDTest.DTOs
{
    public class PhoneRequestDto
    {
        public string Number { get; set; } = string.Empty;
        public string CityCode { get; set; } = string.Empty;
        public string CountryCode { get; set; } = string.Empty;
    }
}

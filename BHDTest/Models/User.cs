using System.ComponentModel.DataAnnotations;

namespace BHDTest.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(255)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MaxLength(255)]
        public string Password { get; set; }

        [Required]
        public DateTime Created { get; set; }

        [Required]
        public DateTime Modified { get; set; }

        [Required]
        public DateTime LastLogin { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        public ICollection<Phone> Phone { get; set; }

    }
}

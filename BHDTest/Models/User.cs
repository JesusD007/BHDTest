using System.ComponentModel.DataAnnotations;

namespace BHDTest.Models
{
    public class User
    {
        public User()
        {
            Id = Guid.NewGuid();
            Created = DateTime.Now;
            Modified = DateTime.Now;
            LastLogin = DateTime.Now;
        }
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public DateTime Created { get; set; }

        [Required]
        public DateTime Modified { get; set; }

        [Required]
        public DateTime LastLogin { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        public ICollection<Phone> Phones { get; set; } = new List<Phone>();

    }
}

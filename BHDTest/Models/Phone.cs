using static BHDTest.Models.User;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BHDTest.Models
{
    public class Phone
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string Number { get; set; }

        [Required]
        [MaxLength(10)]
        public string CityCode { get; set; }

        [Required]
        [MaxLength(10)]
        public string CountryCode { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}

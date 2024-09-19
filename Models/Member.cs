using System.ComponentModel.DataAnnotations;

namespace GymManagementProject.Models
{
    public class Member
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public DateTime Birthday { get; set; }

        [Required]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Id Card Number must be 10 characters long.")]
        public string IdCardNumber { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public DateTime RegistrationDate { get; set; }

        public bool IsDeleted { get; set; }
        public string RegistrationCard { get; set; }
    }
}

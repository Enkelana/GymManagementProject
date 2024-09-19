using System.ComponentModel.DataAnnotations;
namespace GymManagementProject.BAL.DTOs
{
    public class MemberDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [DataType(DataType.Date)]
        public DateTime Birthday { get; set; }

        [StringLength(10, MinimumLength = 10, ErrorMessage = "Id Card Number must be 10 characters long.")]
        public string IdCardNumber { get; set; }
        public string Email { get; set; }

        [DataType(DataType.Date)]
        public DateTime RegistrationDate { get; set; }
        public bool IsDeleted { get; set; }

        public string FullName => $"{FirstName} {LastName}";
        public string RegistrationCard { get; set; }

    }
}

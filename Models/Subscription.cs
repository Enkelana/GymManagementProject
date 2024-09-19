using System.ComponentModel.DataAnnotations;

namespace GymManagementProject.Models
{
    public class Subscription
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Code { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Number of months must be greater than 0.")]
        public int NumberOfMonths { get; set; }

        [Required]
        [Range(1, 7, ErrorMessage = "Week frequency must be a number between 1 and 7.")]
        public int WeekFrequency { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Total number of sessions must be greater than 0.")]
        public int TotalNumberOfSessions { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Total price must be greater than 0.")]
        public decimal TotalPrice { get; set; }

        public bool IsDeleted { get; set; }
    }
}


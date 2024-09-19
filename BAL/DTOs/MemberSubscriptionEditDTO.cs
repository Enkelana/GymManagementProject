using System.ComponentModel.DataAnnotations;
namespace GymManagementProject.BAL.DTOs
{
    public class MemberSubscriptionEditDTO
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public int SubscriptionId { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Original price must be greater than 0.")]
        public decimal OriginalPrice { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Discount Value must be greater than 0 or equal.")]
        public decimal? DiscountValue { get; set; }

        public decimal PaidPrice { get; set; }

        [DataType(DataType.Date)]
        public DateOnly StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateOnly EndDate { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Total number of sessions must be greater than 0.")]
        public int RemainingSessions { get; set; }

        public bool IsDeleted { get; set; }
    }
}

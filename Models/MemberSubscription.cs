using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace GymManagementProject.Models
{
    public class MemberSubscription
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(Member))]
        public int MemberId { get; set; }
        public Member Member { get; set; }

        [Required]
        [ForeignKey(nameof(Subscription))]
        public int SubscriptionId { get; set; }
        public Subscription Subscription { get; set; }

        [Required]
        public decimal OriginalPrice { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Discount Value must be greater than 0 or equal.")]
        public decimal? DiscountValue { get; set; }
        public decimal PaidPrice { get; set; }
        [Required]
        public DateOnly StartDate { get; set; }
        [Required]
        public DateOnly EndDate { get; set; }
        [Required]
        public int RemainingSessions { get; set; }

        public bool IsDeleted { get; set; }
    }
}

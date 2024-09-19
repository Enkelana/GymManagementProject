using GymManagementProject.BAL.DTOs;

namespace GymManagementProject.BAL.Interfaces
{
    public interface IMemberSubscriptionService
    {
        Task<MemberSubscriptionDTO> GetByIdMemberSubscription(int id);
        Task<MemberSubscriptionEditDTO> UpdateMemberSubscription(MemberSubscriptionEditDTO memberSubscriptionDto);
        Task<MemberSubscriptionDTO> CreateMemberSubscription(MemberSubscriptionDTO memberSubscriptionDto);
        Task<IEnumerable<MemberSubscriptionDTO>> SearchMemberSubscription(string searchTerm);
        Task<IEnumerable<MemberSubscriptionDTO>> GetAllMemberSubscriptions();

    }
}

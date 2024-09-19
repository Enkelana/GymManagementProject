using GymManagementProject.BAL.DTOs;

namespace GymManagementProject.BAL.Interfaces
{
    public interface ISubscriptionService
    {
        Task<SubscriptionDTO> GetByIdSubscription(int id);
        Task<SubscriptionDTO> CreateSubscription(SubscriptionDTO subscriptionDto);
        Task<IEnumerable<SubscriptionDTO>> SearchSubscription(string searchTerm);
        Task<SubscriptionEditDTO> UpdateSubscription(SubscriptionEditDTO subscriptionDto);
        Task<IEnumerable<SubscriptionDTO>> GetAllSubscriptions();
        Task<IEnumerable<SubscriptionDTO>> GetAllActiveSubscriptions();
    }
}

using GymManagementProject.BAL.DTOs;

namespace GymManagementProject.BAL.Interfaces
{
    public interface ICheckInService
    {
        Task<CheckInViewModel> CheckInMemberAsync(string registrationCard);
    }
}

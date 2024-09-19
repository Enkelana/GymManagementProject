using GymManagementProject.BAL.DTOs;

namespace GymManagementProject.BAL.Interfaces
{
    public interface IMemberService
    {
        Task<MemberDTO> CreateMember(MemberDTO memberDto);
        Task<IEnumerable<MemberDTO>> SearchMember(string searchTerm);
        Task<MemberDTO> GetByIdMember(int id);
        Task<MemberEditDTO> UpdateMember(MemberEditDTO memberDto);
        Task<IEnumerable<MemberDTO>> GetAllMembers();
        Task<IEnumerable<MemberDTO>> GetAllActiveMembers();
    }
}

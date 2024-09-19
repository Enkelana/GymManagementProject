using System.ComponentModel.DataAnnotations;
namespace GymManagementProject.BAL.DTOs
{
    public class MemberEditDTO
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public bool IsDeleted { get; set; }
    }
}

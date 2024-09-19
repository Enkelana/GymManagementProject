using System.ComponentModel.DataAnnotations;
namespace GymManagementProject.BAL.DTOs
{
    public class CheckInViewModel
    {
        public string RegistrationCard { get; set; }

        public string MemberName { get; set; } = string.Empty;

        public int RemaningSessions { get; set; }

        public string Message { get; set; } = string.Empty;
        public bool IsError { get; set; } = false;
    }
}

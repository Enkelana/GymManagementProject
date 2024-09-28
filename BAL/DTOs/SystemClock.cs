using GymManagementProject.BAL.Interfaces;

namespace GymManagementProject.BAL.DTOs
{
    public class SystemClock :IClock
    {
        DateTime IClock.Now { get { return DateTime.Now.AddHours(-3); } }
    }
}

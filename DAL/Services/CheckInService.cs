using GymManagementProject.BAL.DTOs;
using GymManagementProject.BAL.Interfaces;
using GymManagementProject.Data;
using GymManagementProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymManagementProject.DAL.Services
{
    public class CheckInService : ICheckInService
    {
       
        private readonly ApplicationDbContext context;
        private readonly IClock clock;

        public CheckInService(IClock clock, ApplicationDbContext context)
        {

            this.context = context;
            this.clock = clock;
        }

        public async Task<CheckInViewModel> CheckInMemberAsync(string registrationCard)
        {
            var result = new CheckInViewModel();
            var member = await context.Members
                .FirstOrDefaultAsync(m => m.RegistrationCard == registrationCard && !m.IsDeleted);

            if (member == null)
            {
                result.IsError = true;
                result.Message = "Code is not correct";
                return result;
            }

            var activeSubscription = await context.MemberSubscriptions
                .Include(ms => ms.Subscription)
                .Where(ms => ms.MemberId == member.Id && !ms.IsDeleted &&
                             ms.StartDate <= DateOnly.FromDateTime(DateTime.Now.Date) &&
                             ms.EndDate >= DateOnly.FromDateTime(DateTime.Now.Date))
                .SingleOrDefaultAsync();

            if (activeSubscription == null)
            {
                result.IsError = true;
                result.Message = "No active subscription for this member";
                return result;
            }

            if (activeSubscription.RemainingSessions == 0)
            {
                result.IsError = true;
                result.Message = "No remaining sessions available for this card";
                return result;
            }
            var currentHour = clock.Now.Hour;

             if ((activeSubscription.Subscription.Time == SubscriptionTime.Morning && (currentHour < 6 || currentHour >= 12)) ||
                     (activeSubscription.Subscription.Time == SubscriptionTime.Afternoon && (currentHour < 12 || currentHour >= 18)))
            {
                result.IsError = true;
                result.Message = "This subscription is not valid at this time.";
                return result;
            }

            activeSubscription.RemainingSessions--;
            context.MemberSubscriptions.Update(activeSubscription); 
            await context.SaveChangesAsync();

            result.MemberName = $"{member.FirstName} {member.LastName}";
            result.RemaningSessions = activeSubscription.RemainingSessions;
            result.Message = "Updated member subscription";

            return result;
        }

    }
}
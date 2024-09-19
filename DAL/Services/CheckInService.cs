using GymManagementProject.BAL.DTOs;
using GymManagementProject.BAL.Interfaces;
using GymManagementProject.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymManagementProject.DAL.Services
{
    public class CheckInService : ICheckInService
    {
        private readonly IMemberService memberService;
        private readonly IMemberSubscriptionService memberSubscriptionService;
        private readonly ApplicationDbContext context;

        public CheckInService(IMemberService memberService, IMemberSubscriptionService memberSubscriptionService, ApplicationDbContext context)
        {
            this.memberService = memberService;
            this.memberSubscriptionService = memberSubscriptionService;
            this.context = context;
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
                .SingleOrDefaultAsync(ms => ms.MemberId == member.Id && 
                                            !ms.IsDeleted && 
                                            ms.StartDate <= DateOnly.FromDateTime(DateTime.Now.Date) && 
                                            ms.EndDate >= DateOnly.FromDateTime(DateTime.Now.Date));

            if (activeSubscription == null)
            {
                result.IsError = true;
                result.Message = "No active subscription for this member";
                return result;
            }
            
            if(activeSubscription.RemainingSessions == 0)
            {
                result.IsError = true;
                result.Message = "No remaining sessions available for this card";
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
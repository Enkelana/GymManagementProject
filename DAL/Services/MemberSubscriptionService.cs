using GymManagementProject.BAL.DTOs;
using GymManagementProject.BAL.Interfaces;
using GymManagementProject.Data;
using GymManagementProject.Models;
using Microsoft.EntityFrameworkCore;

namespace GymManagementProject.DAL.Services
{
    public class MemberSubscriptionService : IMemberSubscriptionService
    {
        private readonly ApplicationDbContext context;

        public MemberSubscriptionService(ApplicationDbContext context)
        {
            this.context = context;
        }


        public async Task<MemberSubscriptionDTO> CreateMemberSubscription(MemberSubscriptionDTO memberSubscriptionDto)
        {
            DateOnly todayDate = DateOnly.FromDateTime(DateTime.Today);
     
            if (memberSubscriptionDto.StartDate < todayDate)
            {
                throw new InvalidOperationException("Start date cannot be in the past.");
            }
            try
            {
                var overlappingSubscriptions = await context.MemberSubscriptions
                    .Where(ms => ms.MemberId == memberSubscriptionDto.MemberId && !ms.IsDeleted &&
                                 ((memberSubscriptionDto.StartDate >= ms.StartDate && memberSubscriptionDto.StartDate <= ms.EndDate) ||
                                  (memberSubscriptionDto.EndDate >= ms.StartDate && memberSubscriptionDto.EndDate <= ms.EndDate) ||
                                  (memberSubscriptionDto.StartDate <= ms.StartDate && memberSubscriptionDto.EndDate >= ms.EndDate)))
                    .ToListAsync();

                if (overlappingSubscriptions.Any())
                {
                    throw new InvalidOperationException("Member already has an active subscription during the selected period.");
                }

                var subscription = await context.Subscriptions
                    .FirstOrDefaultAsync(s => s.Id == memberSubscriptionDto.SubscriptionId);

                if (subscription == null)
                {
                    throw new InvalidOperationException("Subscription not found.");
                }

                var endDate = memberSubscriptionDto.StartDate.AddMonths(subscription.NumberOfMonths);
                var paidPrice = subscription.TotalPrice - memberSubscriptionDto.DiscountValue;

                var memberSubscription = new MemberSubscription
                {
                    MemberId = memberSubscriptionDto.MemberId,
                    SubscriptionId = memberSubscriptionDto.SubscriptionId,
                    OriginalPrice = subscription.TotalPrice,
                    DiscountValue = memberSubscriptionDto.DiscountValue,
                    PaidPrice = (decimal)paidPrice,
                    StartDate = memberSubscriptionDto.StartDate,
                    EndDate = endDate,
                    RemainingSessions = subscription.TotalNumberOfSessions,
                    IsDeleted = memberSubscriptionDto.IsDeleted
                };

                context.MemberSubscriptions.Add(memberSubscription);
                await context.SaveChangesAsync();

                return new MemberSubscriptionDTO
                {
                    MemberId = memberSubscription.MemberId,
                    SubscriptionId = memberSubscription.SubscriptionId,
                    OriginalPrice = memberSubscription.OriginalPrice,
                    DiscountValue = memberSubscription.DiscountValue,
                    PaidPrice = memberSubscription.PaidPrice,
                    StartDate = memberSubscription.StartDate,
                    EndDate = memberSubscription.EndDate,
                    RemainingSessions = memberSubscription.RemainingSessions,
                    IsDeleted = memberSubscription.IsDeleted
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public async Task<MemberSubscriptionEditDTO> UpdateMemberSubscription(MemberSubscriptionEditDTO memberSubscriptionDto)
        {
            try
            {
                if (memberSubscriptionDto.DiscountValue < 0)
                {
                    throw new ArgumentException("Discount value cannot be less than 0.");
                }

                var existingMemberSubscription = await context.MemberSubscriptions
                    .FirstOrDefaultAsync(ms => ms.Id == memberSubscriptionDto.Id);

                if (existingMemberSubscription == null)
                {
                    throw new InvalidOperationException("The specified member subscription could not be found.");
                }

                var member = await context.Members
                    .FirstOrDefaultAsync(m => m.Id == existingMemberSubscription.MemberId);

                if (member == null)
                {
                    throw new InvalidOperationException("The associated member could not be found.");
                }

                if (member.IsDeleted)
                {
                    throw new InvalidOperationException("Cannot edit a subscription for a deleted member.");
                }

                var subscription = await context.Subscriptions
                    .FirstOrDefaultAsync(s => s.Id == memberSubscriptionDto.SubscriptionId);

                if (subscription == null)
                {
                    throw new InvalidOperationException("Subscription not found.");
                }

                existingMemberSubscription.DiscountValue = memberSubscriptionDto.DiscountValue;
                existingMemberSubscription.PaidPrice = (decimal)(subscription.TotalPrice - memberSubscriptionDto.DiscountValue);
                existingMemberSubscription.StartDate = memberSubscriptionDto.StartDate;
                existingMemberSubscription.EndDate = memberSubscriptionDto.StartDate.AddMonths(subscription.NumberOfMonths);
                existingMemberSubscription.RemainingSessions = subscription.TotalNumberOfSessions;

                if (!member.IsDeleted)
                {
                    existingMemberSubscription.IsDeleted = memberSubscriptionDto.IsDeleted;
                }

                context.MemberSubscriptions.Update(existingMemberSubscription);
                await context.SaveChangesAsync();

                return new MemberSubscriptionEditDTO
                {
                    Id = existingMemberSubscription.Id,
                    MemberId = existingMemberSubscription.MemberId,
                    SubscriptionId = existingMemberSubscription.SubscriptionId,
                    OriginalPrice = existingMemberSubscription.OriginalPrice,
                    DiscountValue = existingMemberSubscription.DiscountValue,
                    PaidPrice = existingMemberSubscription.PaidPrice,
                    StartDate = existingMemberSubscription.StartDate,
                    EndDate = existingMemberSubscription.EndDate,
                    RemainingSessions = existingMemberSubscription.RemainingSessions,
                    IsDeleted = existingMemberSubscription.IsDeleted
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<MemberSubscriptionDTO> GetByIdMemberSubscription(int id)
        {
            try
            {
                var subscription = await context.MemberSubscriptions
                    .Include(ms => ms.Member)
                    .Include(ms => ms.Subscription)
                    .FirstOrDefaultAsync(ms => ms.Id == id);

                if (subscription == null)
                    return null;

                return new MemberSubscriptionDTO
                {
                    Id = subscription.Id,
                    MemberId = subscription.MemberId,
                    SubscriptionId = subscription.SubscriptionId,
                    OriginalPrice = subscription.OriginalPrice,
                    DiscountValue = subscription.DiscountValue,
                    PaidPrice = subscription.PaidPrice,
                    StartDate = subscription.StartDate,
                    EndDate = subscription.EndDate,
                    RemainingSessions = subscription.RemainingSessions,
                    IsDeleted = subscription.IsDeleted,
                    MemberFullName = $"{subscription.Member.FirstName} {subscription.Member.LastName}",
                    SubscriptionDescription = subscription.Subscription.Description
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<IEnumerable<MemberSubscriptionDTO>> SearchMemberSubscription(string searchTerm)
        {
            try
            {
                var lowerSearchTerm = searchTerm.ToLower();

                var memberSubscriptions = await (
                    from ms in context.MemberSubscriptions
                    join m in context.Members on ms.MemberId equals m.Id
                    join s in context.Subscriptions on ms.SubscriptionId equals s.Id
                    select new
                    {
                        ms.MemberId,
                        ms.SubscriptionId,
                        ms.OriginalPrice,
                        ms.DiscountValue,
                        ms.PaidPrice,
                        ms.StartDate,
                        ms.EndDate,
                        ms.RemainingSessions,
                        ms.IsDeleted,
                        MemberFullName = m.FirstName + " " + m.LastName,
                        SubscriptionDescription = s.Description
                    }).ToListAsync();

                var filteredMemberSubscriptions = memberSubscriptions
                    .Where(ms => ms.MemberFullName.ToLower().Contains(lowerSearchTerm) ||
                                 ms.SubscriptionDescription.ToLower().Contains(lowerSearchTerm))
                    .ToList();

                return filteredMemberSubscriptions.Select(ms => new MemberSubscriptionDTO
                {
                    MemberId = ms.MemberId,
                    SubscriptionId = ms.SubscriptionId,
                    OriginalPrice = ms.OriginalPrice,
                    DiscountValue = ms.DiscountValue,
                    PaidPrice = ms.PaidPrice,
                    StartDate = ms.StartDate,
                    EndDate = ms.EndDate,
                    RemainingSessions = ms.RemainingSessions,
                    IsDeleted = ms.IsDeleted,
                    MemberFullName = ms.MemberFullName,
                    SubscriptionDescription = ms.SubscriptionDescription
                });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IEnumerable<MemberSubscriptionDTO>> GetAllMemberSubscriptions()
        {
            try
            {
                var memberSubscriptions = await context.MemberSubscriptions
                    .Include(ms => ms.Member)
                    .Include(ms => ms.Subscription)
                    .ToListAsync();

                return memberSubscriptions.Select(ms => new MemberSubscriptionDTO
                {
                    Id = ms.Id,
                    MemberId = ms.MemberId,
                    SubscriptionId = ms.SubscriptionId,
                    OriginalPrice = ms.OriginalPrice,
                    DiscountValue = ms.DiscountValue,
                    PaidPrice = ms.PaidPrice,
                    StartDate = ms.StartDate,
                    EndDate = ms.EndDate,
                    RemainingSessions = ms.RemainingSessions,
                    IsDeleted = ms.IsDeleted,
                    MemberFullName = $"{ms.Member.FirstName} {ms.Member.LastName}",
                    SubscriptionDescription = ms.Subscription.Description
                });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
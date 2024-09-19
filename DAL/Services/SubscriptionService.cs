using GymManagementProject.BAL.DTOs;
using GymManagementProject.BAL.Interfaces;
using GymManagementProject.Data;
using GymManagementProject.Models;
using Microsoft.EntityFrameworkCore;

namespace GymManagementProject.DAL.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ApplicationDbContext context;

        public SubscriptionService(ApplicationDbContext context)
        {
            this.context = context;
        }
        public async Task<SubscriptionDTO> GetByIdSubscription(int id)
        {
            try
            {
                var subscription = await context.Subscriptions.FirstAsync(x => x.Id == id);

                if (subscription == null)
                { throw new ArgumentException("The specified subscription could not be found."); }

                return new SubscriptionDTO
                {
                    Id = subscription.Id,
                    Code = subscription.Code,
                    Description = subscription.Description,
                    NumberOfMonths = subscription.NumberOfMonths,
                    WeekFrequency = subscription.WeekFrequency,
                    TotalNumberOfSessions = subscription.TotalNumberOfSessions,
                    TotalPrice = subscription.TotalPrice,
                    IsDeleted = subscription.IsDeleted
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<SubscriptionDTO> CreateSubscription(SubscriptionDTO subscriptionDto)
        {
            try
            {
                var subscription = new Subscription
                {
                    Id = subscriptionDto.Id,
                    Code = subscriptionDto.Code,
                    Description = subscriptionDto.Description,
                    NumberOfMonths = subscriptionDto.NumberOfMonths,
                    WeekFrequency = subscriptionDto.WeekFrequency,
                    TotalNumberOfSessions = subscriptionDto.TotalNumberOfSessions,
                    TotalPrice = subscriptionDto.TotalPrice,
                    IsDeleted = subscriptionDto.IsDeleted
                };

                if (await context.Subscriptions.AnyAsync(m => m.Code == subscription.Code))
                    throw new ArgumentException("A subscription with this Code exists.");

                context.Subscriptions.Add(subscription);
                await context.SaveChangesAsync();

                var subscription_dto = new SubscriptionDTO
                {
                    Id = subscription.Id,
                    Code = subscription.Code,
                    Description = subscription.Description,
                    NumberOfMonths = subscription.NumberOfMonths,
                    WeekFrequency = subscription.WeekFrequency,
                    TotalNumberOfSessions = subscription.TotalNumberOfSessions,
                    TotalPrice = subscription.TotalPrice,
                    IsDeleted = subscription.IsDeleted
                };

                return subscription_dto;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<IEnumerable<SubscriptionDTO>> SearchSubscription(string searchTerm)
        {
            try
            {
                var lowerSearchTerm = searchTerm.ToLower();
                var subscriptions = await context.Subscriptions.ToListAsync();

                var filteredSubscriptions = subscriptions
                    .Where(s => s.Code.ToLower().Contains(lowerSearchTerm) ||
                                s.Description.ToLower().Contains(lowerSearchTerm) ||
                                s.NumberOfMonths.ToString().ToLower().Contains(lowerSearchTerm) ||
                                s.WeekFrequency.ToString().ToLower().Contains(lowerSearchTerm) &&
                                s.IsDeleted == false)
                    .ToList();

                return filteredSubscriptions.Select(s => new SubscriptionDTO
                {
                    Id = s.Id,
                    Code = s.Code,
                    Description = s.Description,
                    NumberOfMonths = s.NumberOfMonths,
                    WeekFrequency = s.WeekFrequency,
                    TotalNumberOfSessions = s.TotalNumberOfSessions,
                    TotalPrice = s.TotalPrice,
                    IsDeleted = s.IsDeleted
                });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<SubscriptionEditDTO> UpdateSubscription(SubscriptionEditDTO subscriptionDto)
        {
            try
            {
                var existingSubscription = await context.Subscriptions.FirstOrDefaultAsync(x => x.Id == subscriptionDto.Id);

                if (existingSubscription == null)
                {
                    throw new ArgumentException("The specified subscription could not be found.");
                }

                existingSubscription.Description = subscriptionDto.Description;
                existingSubscription.NumberOfMonths = subscriptionDto.NumberOfMonths;
                existingSubscription.WeekFrequency = subscriptionDto.WeekFrequency;
                existingSubscription.TotalNumberOfSessions = subscriptionDto.TotalNumberOfSessions;
                existingSubscription.TotalPrice = subscriptionDto.TotalPrice;

                if (subscriptionDto.IsDeleted)
                {
                    existingSubscription.IsDeleted = true;

                    var memberSubscriptions = await context.MemberSubscriptions
                        .Where(ms => ms.SubscriptionId == existingSubscription.Id && ms.IsDeleted == false)
                        .ToListAsync();

                    foreach (var memberSubscription in memberSubscriptions)
                    {
                        memberSubscription.IsDeleted = true;
                    }

                    context.MemberSubscriptions.UpdateRange(memberSubscriptions);
                }
                else
                {
                    existingSubscription.IsDeleted = false;
                }

                context.Subscriptions.Update(existingSubscription);
                await context.SaveChangesAsync();

                return new SubscriptionEditDTO
                {
                    Id = existingSubscription.Id,
                    Description = existingSubscription.Description,
                    NumberOfMonths = existingSubscription.NumberOfMonths,
                    WeekFrequency = existingSubscription.WeekFrequency,
                    TotalNumberOfSessions = existingSubscription.TotalNumberOfSessions,
                    TotalPrice = existingSubscription.TotalPrice,
                    IsDeleted = existingSubscription.IsDeleted
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IEnumerable<SubscriptionDTO>> GetAllSubscriptions()
        {
            try
            {
                var subscriptions = await context.Subscriptions.ToListAsync();

                return subscriptions.Select(s => new SubscriptionDTO
                {
                    Id = s.Id,
                    Code = s.Code,
                    Description = s.Description,
                    NumberOfMonths = s.NumberOfMonths,
                    WeekFrequency = s.WeekFrequency,
                    TotalNumberOfSessions = s.TotalNumberOfSessions,
                    TotalPrice = s.TotalPrice,
                    IsDeleted = s.IsDeleted
                });
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<IEnumerable<SubscriptionDTO>> GetAllActiveSubscriptions()
        {
            try
            {
                var subscriptions = await context.Subscriptions
                    .Where(s => !s.IsDeleted).
                    ToListAsync();

                return subscriptions.Select(s => new SubscriptionDTO
                {
                    Id = s.Id,
                    Code = s.Code,
                    Description = s.Description,
                    NumberOfMonths = s.NumberOfMonths,
                    WeekFrequency = s.WeekFrequency,
                    TotalNumberOfSessions = s.TotalNumberOfSessions,
                    TotalPrice = s.TotalPrice,
                    IsDeleted = s.IsDeleted
                });
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}

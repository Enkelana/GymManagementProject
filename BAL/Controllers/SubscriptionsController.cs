using GymManagementProject.BAL.DTOs;
using GymManagementProject.BAL.Interfaces;
using GymManagementProject.DAL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementProject.BAL.Controllers
{
    [Authorize(Policy = "RequireReceptionistRole")]
    public class SubscriptionsController : Controller
    {
        private readonly ISubscriptionService subscriptionService;
        public SubscriptionsController(ISubscriptionService subscriptionService)
        {
            this.subscriptionService = subscriptionService;
        }
        public async Task<IActionResult> Index(string searchTerm)
        {
            var subscriptions = string.IsNullOrEmpty(searchTerm)
            ? await subscriptionService.GetAllSubscriptions()
            : await subscriptionService.SearchSubscription(searchTerm);

            return View(subscriptions);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(SubscriptionDTO subscriptionDTO)
        {
            if (subscriptionDTO == null)
                throw new ArgumentException("The specified subscription could not be found.");
            try
            {
                var subscription = await subscriptionService.CreateSubscription(subscriptionDTO);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var subscription = await subscriptionService.GetByIdSubscription(id);
                if (subscription == null)
                {
                    throw new ArgumentException("The specified subscription could not be found.");
                }

                return View(subscription);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [HttpPost]
        public async Task<IActionResult> Edit(SubscriptionEditDTO subscriptionDTO)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var updatedSubscription = await subscriptionService.UpdateSubscription(subscriptionDTO);
                    if (updatedSubscription == null)
                    {
                        throw new ArgumentException("The specified subscription could not be found.");
                    }
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            var subscription = await subscriptionService.GetByIdSubscription(subscriptionDTO.Id);
            subscription.Description = subscriptionDTO.Description;
            subscription.NumberOfMonths = subscriptionDTO.NumberOfMonths;
            subscription.WeekFrequency = subscriptionDTO.WeekFrequency;
            subscription.TotalNumberOfSessions = subscriptionDTO.TotalNumberOfSessions;
            subscription.TotalPrice = subscriptionDTO.TotalPrice;
            subscription.IsDeleted = subscriptionDTO.IsDeleted;
            subscription.Time = subscriptionDTO.Time;

            return View(subscription);
        }
    }
}


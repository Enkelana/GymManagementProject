using GymManagementProject.BAL.DTOs;
using GymManagementProject.BAL.Interfaces;
using GymManagementProject.DAL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymManagementProject.BAL.Controllers
{
    [Authorize(Policy = "RequireReceptionistRole")]
    public class MemberSubscriptionsController : Controller
    {
        private readonly IMemberSubscriptionService memberSubscriptionService;
        private readonly ISubscriptionService subscriptionService;
        private readonly IMemberService memberService;

        public MemberSubscriptionsController(IMemberSubscriptionService memberSubscriptionService, ISubscriptionService subscriptionService, IMemberService memberService)
        {
            this.memberSubscriptionService = memberSubscriptionService;
            this.subscriptionService = subscriptionService;
            this.memberService = memberService;
        }

        public async Task<IActionResult> Index(string searchTerm)
        {
            var memberSubscriptions = string.IsNullOrEmpty(searchTerm)
              ? await memberSubscriptionService.GetAllMemberSubscriptions()
              : await memberSubscriptionService.SearchMemberSubscription(searchTerm);

            return View(memberSubscriptions);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var members = await memberService.GetAllActiveMembers();
            var subscriptions = await subscriptionService.GetAllActiveSubscriptions();

            ViewBag.Members = new SelectList(members, "Id", "FullName");
            ViewBag.Subscriptions = new SelectList(subscriptions, "Id", "Description");

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(MemberSubscriptionDTO memberSubscriptionDTO)
        {
            if (memberSubscriptionDTO == null)
                throw new ArgumentException("The specified member subscription could not be found.");

            try
            {
                var memberSubscription = await memberSubscriptionService.CreateMemberSubscription(memberSubscriptionDTO);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var memberSubscription = await memberSubscriptionService.GetByIdMemberSubscription(id);
            if (memberSubscription == null)
            {
                throw new ArgumentException("The specified member subscription could not be found.");
            }

            var model = new MemberSubscriptionEditDTO
            {
                Id = memberSubscription.Id,
                MemberId = memberSubscription.MemberId,
                SubscriptionId = memberSubscription.SubscriptionId,
                OriginalPrice = memberSubscription.OriginalPrice,
                DiscountValue = memberSubscription.DiscountValue,
                StartDate = memberSubscription.StartDate,
                EndDate = memberSubscription.EndDate,
                RemainingSessions = memberSubscription.RemainingSessions,
                IsDeleted = memberSubscription.IsDeleted
            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(MemberSubscriptionEditDTO memberSubscriptionEditDTO)
        {
            if (!ModelState.IsValid)
            {
                return View(memberSubscriptionEditDTO);
            }

            try
            {
                var updatedSubscription = await memberSubscriptionService.UpdateMemberSubscription(memberSubscriptionEditDTO);
                if (updatedSubscription == null)
                {
                    throw new ArgumentException("The specified member subscription could not be found.");
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}

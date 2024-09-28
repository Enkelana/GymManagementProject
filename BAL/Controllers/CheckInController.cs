using GymManagementProject.BAL.DTOs;
using GymManagementProject.BAL.Interfaces;
using GymManagementProject.DAL.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GymManagementProject.Controllers
{
    public class CheckInController : Controller
    {
        private readonly ICheckInService checkInService;

        public CheckInController(ICheckInService checkInService)
        {
            this.checkInService = checkInService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CheckIn(string registrationCard)
        {
            var viewModel = new CheckInViewModel();

            try
            {
                if (string.IsNullOrEmpty(registrationCard))
                {
                    ModelState.AddModelError("", "Registration card is required.");
                    return View("Index", viewModel);
                }

                viewModel = await checkInService.CheckInMemberAsync(registrationCard.Trim());

                if (viewModel.IsError)
                {
                    TempData["ErrorMessage"] = viewModel.Message;
                    return View("Index", viewModel);
                }

                TempData["SuccessMessage"] = viewModel.Message;
                return View("Index", viewModel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while processing your request.");
                return View("Index", viewModel);
            }
        }

    }
}

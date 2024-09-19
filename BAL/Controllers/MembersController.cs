using GymManagementProject.BAL.DTOs;
using GymManagementProject.BAL.Interfaces;
using GymManagementProject.DAL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementProject.BAL.Controllers
{
    [Authorize(Policy = "RequireReceptionistRole")]
    public class MembersController : Controller
    {
        private readonly IMemberService memberService;
        public MembersController(IMemberService memberService)
        {
            this.memberService = memberService;
        }
        public async Task<IActionResult> Index(string searchTerm)
        {
            var members = string.IsNullOrEmpty(searchTerm)
                ? await memberService.GetAllMembers()
                : await memberService.SearchMember(searchTerm);

            return View(members);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(MemberDTO memberDto)
        {
            if (memberDto == null)
                throw new ArgumentException("The specified member could not be found.");
            try
            {
                var member = await memberService.CreateMember(memberDto);
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
                var member = await memberService.GetByIdMember(id);
                if (member == null)
                {
                    throw new ArgumentException("The specified member could not be found.");
                }

                return View(member);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [HttpPost]
        public async Task<IActionResult> Edit(MemberEditDTO memberDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var updatedMember = await memberService.UpdateMember(memberDto);
                    if (updatedMember == null)
                    {
                        throw new ArgumentException("The specified member could not be found.");
                    }

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            var member = await memberService.GetByIdMember(memberDto.Id);
            member.Email = memberDto.Email;
            member.IsDeleted = memberDto.IsDeleted;

            return View(member);
        }

    }
}
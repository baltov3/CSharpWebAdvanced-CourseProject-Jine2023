using HouseRenting.Services.Data.Interfaces;
using HouseRentingSystem.Web.Infrastructure.Extensions;
using HouseRentingSystem.Web.ViewModels.Agent;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static HouseRentingSystem.Coomon.NotificationsMessagesConstants;
namespace HouseRentingSystem.Web.Controllers
{
    [Authorize]
    public class AgentController : Controller
    {
        private readonly IAgentService agentService;

        public AgentController(IAgentService service)
        {
            this.agentService = service;
        }

        [HttpGet]
        public async Task< IActionResult> Become()
        {
            string userid = this.User.GetId();  
            bool isAgent = await this.agentService.AgentexistByUserId(userid);
            if (isAgent) 
            {
                TempData[ErrorMessage] = "You are already an agent";
             return   RedirectToAction(nameof(HomeController.Index), "Home");
            }
            return View();
            
        }
        [HttpPost]
        public async Task<IActionResult> Become(BecomeAgentFormModel model)
        {
            string userid = this.User.GetId();
            bool isAgent = await this.agentService.AgentexistByUserId(userid);
            if (isAgent)
            {
                TempData[ErrorMessage] = "You are already an agent";
                return RedirectToAction(nameof(HomeController.Index), "Home");

            }
            bool isPhoneNumberTaken = await this.agentService.UserWithPhoneNumberExists(model.PhoneNumber);
            if (isPhoneNumberTaken)
            {
                ModelState.AddModelError(nameof(model.PhoneNumber),"Agent with providen phonenumber already exist!");
            }
            if (!this.ModelState.IsValid)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            bool activeRents = await this.agentService.UserHasRents(userid);
            if (activeRents)
            {
                TempData[ErrorMessage] = "You must dont have any active rents in order to become an agent!";
               return this.RedirectToAction("Mine", "House");
            }

            try
            {
                await this.agentService.Create(userid, model);

            }
            catch (Exception)
            {

                this.TempData[ErrorMessage] = "Unexepected error occured while registering you as agent!";
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            return this.RedirectToAction(nameof(HouseController.All), "House");
        }

    }
}

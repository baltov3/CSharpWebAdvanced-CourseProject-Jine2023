using HouseRenting.Services.Data.Interfaces;
using HouseRentingSystem.Web.Infrastructure.Extensions;
using HouseRentingSystem.Web.ViewModels.House;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static HouseRentingSystem.Coomon.NotificationsMessagesConstants;

namespace HouseRentingSystem.Web.Controllers
{
    [Authorize]
    public class HouseController : Controller
    {
       private readonly ICategoryService _categoryService;
        private readonly IAgentService agentService;

        public HouseController(ICategoryService categoryService, IAgentService agentService)
        {
            _categoryService = categoryService;
            this.agentService = agentService;
        }

        [AllowAnonymous]
        public async Task<IActionResult> All()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            bool isAgent = await this.agentService.AgentexistByUserId(this.User.GetId()!);
            if (!isAgent)
            {
                TempData[ErrorMessage] = "Ypu must be an agent!";
                return RedirectToAction(nameof(AgentController.Become), "Agent");
            }
            HouseFormModel model = new HouseFormModel()
            {
                Categories = await this._categoryService.AllCategoriesAsync()

            };

            return View(model);

        }
    }
}

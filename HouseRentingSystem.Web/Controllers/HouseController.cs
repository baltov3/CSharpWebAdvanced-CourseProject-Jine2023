using HouseRenting.Services.Data.Interfaces;
using HouseRentingSystem.Services.Data.Models.House;
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
        private readonly IHouseService houseService;

        public HouseController(ICategoryService categoryService, IAgentService agentService, IHouseService houseService)
        {
            _categoryService = categoryService;
            this.agentService = agentService;
            this.houseService = houseService;
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> All([FromQuery]AllHousesQueryModel queryModel)
        {
            AllHouseFilteredAndPagedServiceModel serviceModel = await houseService.AllAsync(queryModel);
            queryModel.Houses = serviceModel.Houses;
            queryModel.TotalHouses = serviceModel.TotalHousesCount;
            queryModel.Categories = await this._categoryService.AllCategoryNamesAsync();

            return this.View(queryModel);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            bool isAgent = await this.agentService.AgentexistByUserId(this.User.GetId()!);
            if (!isAgent)
            {
                TempData[ErrorMessage] = "You must be an agent!";
                return RedirectToAction(nameof(AgentController.Become), "Agent");
            }
            HouseFormModel model = new HouseFormModel()
            {
                Categories = await this._categoryService.AllCategoriesAsync()

            };

            return View(model);

        }
        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            
            HouseDetailsViewModel? houseDetailsViewModel=await this.houseService.GetDetailsByIdAsync(id);
            if (houseDetailsViewModel==null)
            {
                TempData[ErrorMessage] = "House does not exist";
                return RedirectToAction(nameof(HouseController.All), "House");
            }

            return View(houseDetailsViewModel);

        }

        [HttpPost]
        public async Task<IActionResult> Add(HouseFormModel model)
        {
            bool isAgent = await this.agentService.AgentexistByUserId(this.User.GetId()!);
            if (!isAgent)
            {
                this.TempData[ErrorMessage] = "Ypu must be an agent!";
                return RedirectToAction(nameof(AgentController.Become), "Agent");
            }
           
            bool ctaegoryExist = await this._categoryService.ExistByIdAsync(model.CategoryId);
            if (!ctaegoryExist)
            {
                this.ModelState.AddModelError(nameof(model.CategoryId), "Selected category does not exist");
               
            }
            if (!this.ModelState.IsValid)
            {
                model.Categories = await this._categoryService.AllCategoriesAsync();
                return View(model);
            }

            try
            {
                string? agentId = await agentService.GetAgentIdByuserIdAsync(this.User.GetId()!);
               await this.houseService.CreateAsync(model, agentId!);
            }
            catch (Exception )
            {

                this.ModelState.AddModelError(string.Empty, "Unexpected error occured while you try to add your new house!");
                model.Categories = await this._categoryService.AllCategoriesAsync();
                return View(model);
            }
            return this.RedirectToAction(nameof(HouseController.All), "House");
        }
        [HttpGet]
        public async Task<IActionResult> Mine( )
        {
            List<HouseAllViewModel> houseAllViewModels = new List   <HouseAllViewModel>();
            bool isAgent = await this.agentService.AgentexistByUserId(this.User.GetId()!);
            string userId = User.GetId();
            if (isAgent)
            {
                string? agentId = await this.agentService.GetAgentIdByuserIdAsync(userId);
                houseAllViewModels.AddRange(await this.houseService.AllByAgentIdAsync(agentId!));
            }
            else
            {
                houseAllViewModels.AddRange(await this.houseService.AllByUserIdAsync(userId!));
            }
            return View(houseAllViewModels);
        }

    }
}

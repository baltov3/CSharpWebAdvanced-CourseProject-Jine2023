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
             bool houseExist = await this.houseService.ExistById(id);
            if (!houseExist) 
            {
                TempData[ErrorMessage] = "House does not exist";
                return RedirectToAction(nameof(HouseController.All), "House");
            }
            HouseDetailsViewModel houseDetailsViewModel=await this.houseService.GetDetailsByIdAsync(id);
            

            return View(houseDetailsViewModel);

        }
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {

            bool houseExist = await this.houseService.ExistById(id);
            if (!houseExist)
            {
                TempData[ErrorMessage] = "House does not exist";
                return RedirectToAction(nameof(HouseController.All), "House");
            }

            bool isAgent = await this.agentService.AgentexistByUserId(this.User.GetId()!);
            if (!isAgent)
            {
                TempData[ErrorMessage] = "You must become an agent";
                return RedirectToAction(nameof(AgentController.Become), "Agent");
            }
            string? agentId = await this.agentService.GetAgentIdByuserIdAsync(this.User.GetId()!);
            bool isAgentOwner = await this.houseService.IsAgentWithIdOwnerOfHouseIdAsync(id,agentId!);
            if (!isAgentOwner)
            {
                TempData[ErrorMessage] = "You must be agent owner of the house to edit!";
                return RedirectToAction(nameof(HouseController.Mine), "House");
            }
            try
            {
                HouseFormModel model = await this.houseService.GetHouseForEditByIdAsync(id);
                model.Categories = await this._categoryService.AllCategoriesAsync();

                return View(model);
            }
            catch (Exception)
            {
                

                TempData[ErrorMessage] = "Unexpected error occured while trying to edit update the house";
                return RedirectToAction(nameof(HouseController.All), "House");
            }
          

        }
        [HttpPost]
        public async Task<IActionResult> Edit(string id,HouseFormModel houseFormModel)
        {
            if (!ModelState.IsValid)
            {
                houseFormModel.Categories = await this._categoryService.AllCategoriesAsync();
                return View(houseFormModel);
            }
            bool houseExist = await this.houseService.ExistById(id);
            if (!houseExist)
            {
                TempData[ErrorMessage] = "House does not exist";
                return RedirectToAction(nameof(HouseController.All), "House");
            }

            bool isAgent = await this.agentService.AgentexistByUserId(this.User.GetId()!);
            if (!isAgent)
            {
                TempData[ErrorMessage] = "You must become an agent";
                return RedirectToAction(nameof(AgentController.Become), "Agent");
            }
            string? agentId = await this.agentService.GetAgentIdByuserIdAsync(this.User.GetId()!);
            bool isAgentOwner = await this.houseService.IsAgentWithIdOwnerOfHouseIdAsync(id, agentId!);
            if (!isAgentOwner)
            {
                TempData[ErrorMessage] = "You must be agent owner of the house to edit!";
                return RedirectToAction(nameof(HouseController.Mine), "House");
            }
            try
            {
              await  this.houseService.EditHouseByIdFormModel(id, houseFormModel);
            }
            catch (Exception)
            {
                this.ModelState.AddModelError(string.Empty, "Unexpected error occured while trying to edit update the house");
                houseFormModel.Categories = await this._categoryService.AllCategoriesAsync();
                return View(houseFormModel);
            }
            this.TempData[SuccessMessage] = "House was added successfully!";
            return RedirectToAction("Details", "House",new {id});

        }      
        [HttpGet]     
        public async Task<IActionResult> Delete(string id)
        {
            bool houseExist = await this.houseService.ExistById(id);
            if (!houseExist)
            {
                TempData[ErrorMessage] = "House does not exist";

                 return this.RedirectToAction("All", "House");
            }

            bool isUserAgent = await this.agentService
                .AgentexistByUserId(this.User.GetId()!);
            if (!isUserAgent)
            {
                this.TempData[ErrorMessage] = "You must become an agent in order to edit house info!";

                return this.RedirectToAction("Become", "Agent");
            }

            string? agentId =
                await this.agentService.GetAgentIdByuserIdAsync(this.User.GetId()!);
            bool isAgentOwner = await this.houseService
                .IsAgentWithIdOwnerOfHouseIdAsync(id, agentId!);
            if (!isAgentOwner)
            {
                this.TempData[ErrorMessage] = "You must be the agent owner of the house you want to edit!";

                return this.RedirectToAction("Mine", "House");
            }

           
                HousePreDeleteDetailsViewModel viewModel =
                    await this.houseService.GetHouseForDeleteDetailsByIdAsync(id);

                return this.View(viewModel);
            
           
             
            
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id, HousePreDeleteDetailsViewModel model)
        {
            bool houseExists = await this.houseService
                .ExistById(id);
            if (!houseExists)
            {
                this.TempData[ErrorMessage] = "House with the provided id does not exist!";

                return this.RedirectToAction("All", "House");
            }

            bool isUserAgent = await this.agentService
                .AgentexistByUserId(this.User.GetId()!);
            if (!isUserAgent)
            {
                this.TempData[ErrorMessage] = "You must become an agent in order to edit house info!";

                return this.RedirectToAction("Become", "Agent");
            }

            string? agentId =
                await this.agentService.GetAgentIdByuserIdAsync(this.User.GetId()!);
            bool isAgentOwner = await this.houseService
                .IsAgentWithIdOwnerOfHouseIdAsync(id, agentId!);
            if (!isAgentOwner)
            {
                this.TempData[ErrorMessage] = "You must be the agent owner of the house you want to edit!";

                return this.RedirectToAction("Mine", "House");
            }

            
            
                await this.houseService.GetHouseForDeleteDetailsByIdAsync(id);

                this.TempData[WarningMessage] = "The house was successfully deleted!";
                return this.RedirectToAction("Mine", "House");
            
            
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
                this.TempData[SuccessMessage] = "House was added successfully!";
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
        [HttpPost]
        public async Task<IActionResult> Rent(string id)
        {
            bool houseExists = await this.houseService
                .ExistById(id);
            if (!houseExists)
            {
                this.TempData[ErrorMessage] = "House with the provided id does not exist!";

                return this.RedirectToAction("All", "House");
            }
            bool isRented = await this.houseService.IsRentedByIdAsync(id);
            if (isRented)
            {
                this.TempData[ErrorMessage] = "House ia already rented!";

                return this.RedirectToAction("All", "House");
            }

            bool isAgent = await this.agentService.AgentexistByUserId(this.User.GetId()!);
            if (isAgent)
            {
                this.TempData[ErrorMessage] = "Agent can not rent houses!";
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            try
            {
                await this.houseService.RentHouseAsync(id, this.User.GetId());
            }
            catch (Exception)
            {

                this.GeneralError();
            }
            return this.RedirectToAction(nameof(HouseController.Mine), "House");
        }
        [HttpPost]
        public async Task<IActionResult> Leave(string id)
        {
            bool houseExists = await this.houseService
                .ExistById(id);
            if (!houseExists)
            {
                this.TempData[ErrorMessage] = "House with the provided id does not exist!";

                return this.RedirectToAction("All", "House");
            }
            bool isRented = await this.houseService.IsRentedByIdAsync(id);
            if (isRented)
            {
                this.TempData[ErrorMessage] = "House is not rented!";

                return this.RedirectToAction("Mine", "House");
            }
            bool isCurrentRenterOfHouse = await this.houseService.IsRentedByUserWithIdAsync(id, this.User.GetId());
            if (!isCurrentRenterOfHouse)
            {
                this.TempData[ErrorMessage] = "You must be a renter of the house!";

                return this.RedirectToAction("All", "House");
            }


            try
            {
                await this.houseService.LeaveHouseAsync(id);
            }
            catch (Exception)
            {

               return this.GeneralError();
            }
            return this.RedirectToAction(nameof(HouseController.Mine), "House");
        }
        private IActionResult GeneralError()
        {
            this.TempData[ErrorMessage] =
                "Unexpected error occurred! Please try again later or contact administrator";

            return this.RedirectToAction(nameof(HomeController.Index), "Home");
        }

    }
}

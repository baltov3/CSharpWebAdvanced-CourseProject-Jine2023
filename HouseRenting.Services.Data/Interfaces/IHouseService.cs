﻿using HouseRentingSystem.Services.Data.Models.House;
using HouseRentingSystem.Web.ViewModels.Home;
using HouseRentingSystem.Web.ViewModels.House;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseRenting.Services.Data.Interfaces
{
    public interface IHouseService
    {
        Task<IEnumerable<IndexViewModel>> LastThreeHousesAsync();
        Task CreateAsync(HouseFormModel model,string agentId);
        Task<AllHouseFilteredAndPagedServiceModel> AllAsync(AllHousesQueryModel queryModel);
        Task<IEnumerable<HouseAllViewModel>> AllByAgentIdAsync(string agentId);
        Task<IEnumerable<HouseAllViewModel>> AllByUserIdAsync(string userId);

        Task<HouseDetailsViewModel> GetDetailsByIdAsync(string houseId);
        Task<bool> ExistById(string houseId);
        Task<HouseFormModel> GetHouseForEditByIdAsync(string houseId);

        Task<bool> IsAgentWithIdOwnerOfHouseIdAsync(string houseId, string agentId);

        Task EditHouseByIdFormModel (string houseId,HouseFormModel houseFormModel);
    }
}

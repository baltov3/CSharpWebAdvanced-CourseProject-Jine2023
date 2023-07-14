using HouseRenting.Services.Data.Interfaces;
using HouseRentingSystem.Data.Models;
using HouseRentingSystem.Services.Data.Models.House;
using HouseRentingSystem.Web.Data;
using HouseRentingSystem.Web.ViewModels.Agent;
using HouseRentingSystem.Web.ViewModels.Home;
using HouseRentingSystem.Web.ViewModels.House;
using HouseRentingSystem.Web.ViewModels.House.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseRenting.Services.Data
{
    public class HouseService : IHouseService
    {
        private readonly HouseRentingDbContext dbContext;

        public HouseService(HouseRentingDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<AllHouseFilteredAndPagedServiceModel> AllAsync(AllHousesQueryModel queryModel)
        {
            IQueryable<House> houses = this.dbContext.Houses.AsQueryable();
            if (!string.IsNullOrWhiteSpace(queryModel.Category))
            {
                houses = houses.Where(h => h.Category.Name == queryModel.Category);
            }
            if (!string.IsNullOrWhiteSpace(queryModel.SerchString))
            {
                string wildCard = $"%{queryModel.SerchString.ToLower()}%";
                houses = houses.Where(h => EF.Functions.Like(h.Title,wildCard)|| EF.Functions.Like(h.Address, wildCard)||
                EF.Functions.Like(h.Description, wildCard));
            }
            if (!string.IsNullOrWhiteSpace(queryModel.SerchString))
            {
                string wildCard = $"%{queryModel.SerchString.ToLower()}%";
                houses = houses.Where(h => EF.Functions.Like(h.Title, wildCard) || EF.Functions.Like(h.Address, wildCard) ||
                EF.Functions.Like(h.Description, wildCard));
            }
            houses = queryModel.HouseSorting switch
            {
                HouseSorting.Newest => houses.OrderByDescending(h=>h.CreatedOn),
                HouseSorting.Oldest => houses.OrderBy(h => h.CreatedOn),
                HouseSorting.PriceDescending => houses.OrderByDescending(h => h.PricePerMonth),
                HouseSorting.PriceAscending => houses.OrderBy(h => h.PricePerMonth),
               _ => houses.OrderBy(h=>h.RenterId!=null).
                ThenByDescending(h => h.CreatedOn),

            };
            IEnumerable<HouseAllViewModel> allHouses =await houses.
                Where(h => h.IsActive).
                Skip((queryModel.CurrentPage-1)*queryModel.HousesPerPage).
                Take(queryModel.HousesPerPage).Select(h=> new HouseAllViewModel()
                {
                          Id=h.Id.ToString(),
                        Title = h.Title,
                        Address = h.Address,
                        ImageUrl = h.ImageUrl,
                        PricePerMonth = h.PricePerMonth,
                        IsRented = h.RenterId.HasValue

                }).ToArrayAsync();
            int totalHouses =   houses.Count();
            return new AllHouseFilteredAndPagedServiceModel()
            {
                TotalHousesCount = totalHouses,
                Houses = allHouses
            };
        }

        public async Task<IEnumerable<HouseAllViewModel>> AllByAgentIdAsync(string agentId)
        {
            IEnumerable<HouseAllViewModel> allAgentHouses = await this.dbContext.Houses.Where(a => a.AgentId.ToString() == agentId&&a.IsActive).
                Select(h => new HouseAllViewModel()
                {
                    Id = h.Id.ToString(),
                    Title = h.Title,
                    Address = h.Address,
                    ImageUrl = h.ImageUrl,
                    PricePerMonth = h.PricePerMonth,
                    IsRented=h.RenterId.HasValue
                }).ToArrayAsync();
            return allAgentHouses;
        }

        public async Task<IEnumerable<HouseAllViewModel>> AllByUserIdAsync(string userId)
        {
            IEnumerable<HouseAllViewModel> allUserHouses = await this.dbContext.Houses
                .Where(a =>a.IsActive&&a.RenterId.HasValue&&a.RenterId.ToString() == userId).
                Select(h => new HouseAllViewModel()
                {
                    Id = h.Id.ToString(),
                    Title = h.Title,
                    Address = h.Address,
                    ImageUrl = h.ImageUrl,
                    PricePerMonth = h.PricePerMonth,
                    IsRented = h.RenterId.HasValue
                }).ToArrayAsync();
            return allUserHouses;
        }

        public async Task CreateAsync(HouseFormModel model, string agentId)
        {
           House house = new House()
           {
               Title = model.Title,
               Address = model.Address,
               ImageUrl = model.ImageUrl,
               Description = model.Description,
               PricePerMonth = model.PricePerMonth,
               CategoryId = model.CategoryId,
               AgentId = Guid.Parse(agentId)

               
           };
            await dbContext.Houses.AddAsync(house);
            await dbContext.SaveChangesAsync();

        }

        public async Task DeleteHouseByIdAsync(string houseId)
        {
            House houseToDelete = await this.dbContext
                .Houses
                .Where(h => h.IsActive)
                .FirstAsync(h => h.Id.ToString() == houseId);

            houseToDelete.IsActive = false;

            await this.dbContext.SaveChangesAsync();
        }

        public  async Task EditHouseByIdFormModel(string houseId, HouseFormModel houseFormModel)
        {
           House house = await this.dbContext.Houses.Where(h=>h.IsActive).FirstAsync(h=>h.Id.ToString()==houseId);
            house.Title=houseFormModel.Title;
            house.Address=houseFormModel.Address;
            house.ImageUrl=houseFormModel.ImageUrl;
            house.Description=houseFormModel.Description;
            house.PricePerMonth=houseFormModel.PricePerMonth;
            house.CategoryId =houseFormModel.CategoryId;

            await this.dbContext.SaveChangesAsync();

        }

        public async Task<bool> ExistById(string houseId)
        {
            bool result = await this.dbContext.Houses.Where(h=>h.IsActive).AnyAsync(h => h.Id.ToString() == houseId);
            return result;
        }

        public async Task<HouseDetailsViewModel> GetDetailsByIdAsync(string houseId)
        {
            House house = await this.dbContext.Houses.
                Include(h=>h.Category).
                Include(h=>h.Agent).
                ThenInclude(h=>h.User)
                .Where(h=>h.IsActive).FirstAsync(h=>h.Id.ToString()==houseId);
            
            return new HouseDetailsViewModel()
            {
                Id = houseId,
                Title = house.Title,
                Address = house.Address,
                ImageUrl = house.ImageUrl,
                Description = house.Description,
                PricePerMonth = house.PricePerMonth,
                IsRented = house.RenterId.HasValue,
                Category = house.Category.Name,
                Agent = new AgentInfoOnHouseVieModel()
                {
                    Email = house.Agent.User.Email,
                    PhoneNumber= house.Agent.PhoneNumber,
                }

            };
        }

        public async Task<HouseFormModel> GetHouseForEditByIdAsync(string houseId)
        {
            var house = await this.dbContext.Houses.
                  Include(h => h.Category)                
                  .Where(h => h.IsActive).FirstAsync(h => h.Id.ToString() == houseId);
            return new HouseFormModel()
            {
                Title= house.Title,
                Address= house.Address,
                ImageUrl= house.ImageUrl,
                Description = house.Description,
                PricePerMonth = house.PricePerMonth,
                CategoryId=house.CategoryId
            };


        }

        public async Task<HousePreDeleteDetailsViewModel> GetHouseForDeleteDetailsByIdAsync(string houseId)
        {
            House house = await this.dbContext.Houses.
               Where(h => h.IsActive)
               .FirstAsync(h => h.Id.ToString() == houseId);
            return new HousePreDeleteDetailsViewModel()
            {
             
                Title = house.Title,
                Address= house.Address,
                ImageUrl = house.ImageUrl
            };
        }

        public async Task<bool> IsAgentWithIdOwnerOfHouseIdAsync(string houseId, string agentId)
        {
            House house = await this.dbContext.Houses.
                Where(h=>h.IsActive)
                .FirstAsync(h=>h.Id.ToString() == houseId);
            return house.AgentId.ToString() == agentId;
        }

        public async Task<IEnumerable<IndexViewModel>> LastThreeHousesAsync()
        {
            IEnumerable<IndexViewModel> lastthreeHouses = await this.dbContext.Houses.Where(h=>h.IsActive).OrderByDescending(h=>h.CreatedOn).Take(3).Select(
                h=> new IndexViewModel
                {
                    Id=h.Id.ToString(),
                    Title=h.Title,
                    ImageUrl=h.ImageUrl
                }).ToArrayAsync();
            return lastthreeHouses;
        }

        public async Task<bool> IsRentedByIdAsync(string houseId)
        {
            House house = await this.dbContext.Houses.
                 FirstAsync(h => h.Id.ToString() == houseId);
            return house.RenterId.HasValue;
        }

        public async Task RentHouseAsync(string houseId, string userId)
        {
            House house = await this.dbContext.Houses.
                 FirstAsync(h => h.Id.ToString() == houseId);
            house.RenterId=Guid.Parse(userId);

            await this.dbContext.SaveChangesAsync();
        }
    }
}

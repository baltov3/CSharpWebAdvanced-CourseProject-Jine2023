using HouseRenting.Services.Data.Interfaces;
using HouseRentingSystem.Data.Models;
using HouseRentingSystem.Services.Data.Models.House;
using HouseRentingSystem.Web.Data;
using HouseRentingSystem.Web.ViewModels.Home;
using HouseRentingSystem.Web.ViewModels.House;
using HouseRentingSystem.Web.ViewModels.House.Enums;
using Microsoft.EntityFrameworkCore;
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
                HouseSorting.Newest => houses.OrderBy(h=>h.CreatedOn),
                HouseSorting.Oldest => houses.OrderByDescending(h => h.CreatedOn),
                HouseSorting.PriceDescending => houses.OrderByDescending(h => h.PricePerMonth),
                HouseSorting.PriceAscending => houses.OrderBy(h => h.PricePerMonth),
               _ => houses.OrderBy(h=>h.RenterId!=null).
                ThenByDescending(h => h.CreatedOn),

            };
            IEnumerable<HouseAllViewModel> allHouses =await houses.Skip((queryModel.CurrentPage-1)*queryModel.HousesPerPage).
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

        public async Task<IEnumerable<IndexViewModel>> LastThreeHousesAsync()
        {
            IEnumerable<IndexViewModel> lastthreeHouses = await this.dbContext.Houses.OrderByDescending(h=>h.CreatedOn).Take(3).Select(
                h=> new IndexViewModel
                {
                    Id=h.Id.ToString(),
                    Title=h.Title,
                    ImageUrl=h.ImageUrl
                }).ToArrayAsync();
            return lastthreeHouses;
        }
    }
}

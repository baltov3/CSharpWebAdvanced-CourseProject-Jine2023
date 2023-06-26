using HouseRenting.Services.Data.Interfaces;
using HouseRentingSystem.Web.Data;
using HouseRentingSystem.Web.ViewModels.Home;
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

using HouseRentingSystem.Web.ViewModels.House;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseRentingSystem.Services.Data.Models.House
{
    public class AllHouseFilteredAndPagedServiceModel
    {
        public AllHouseFilteredAndPagedServiceModel()
        {
            Houses = new HashSet<HouseAllViewModel>();
        }

        public int TotalHousesCount { get; set; }
        public IEnumerable<HouseAllViewModel> Houses { get; set; }
    }
}

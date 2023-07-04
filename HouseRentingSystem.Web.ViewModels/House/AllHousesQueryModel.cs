using HouseRentingSystem.Web.ViewModels.Category;
using HouseRentingSystem.Web.ViewModels.House.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseRentingSystem.Web.ViewModels.House
{
    public class AllHousesQueryModel
    {
        public AllHousesQueryModel()
        {
            this.CurrentPage = 1;
            this.HousesPerPage = 3;
            Categories = new HashSet<string>();
            Houses = new HashSet<HouseAllViewModel>();
        }

        public string? Category { get; set; }
        [Display(Name = "Serch by word")]
        public string? SerchString { get; set; }
        [Display(Name = "Sort by")]
        public HouseSorting  HouseSorting { get; set; }
        public int CurrentPage { get; set; }
        public int HousesPerPage { get; set; }
        public int TotalHouses   { get; set; }
        public IEnumerable<string> Categories { get; set; } = null!;

        public IEnumerable<HouseAllViewModel> Houses { get; set; } = null!;
    }
}

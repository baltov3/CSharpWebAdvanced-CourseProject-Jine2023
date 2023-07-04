using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseRentingSystem.Web.ViewModels.House
{
    public class HouseAllViewModel
    {
        public string Id { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Address { get; set; } = null!;
        [Display(Name = "Image link")]
        public string ImageUrl { get; set; } = null!;
        [Display(Name = "Month Price")]
        public decimal PricePerMonth { get; set; } 
        public bool IsRented { get; set; }


    }
}

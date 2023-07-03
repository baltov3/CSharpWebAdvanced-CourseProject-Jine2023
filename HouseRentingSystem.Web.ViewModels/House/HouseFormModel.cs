using HouseRentingSystem.Web.ViewModels.Category;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HouseRentingSystem.Coomon.EntityValidationConstants.House;
namespace HouseRentingSystem.Web.ViewModels.House
{
    public class HouseFormModel
    {
        public HouseFormModel()
        {
            Categories = new HashSet<HouseSelectCategoryFormModel>();
        }
        [Required]
        [StringLength(TitleMaxLenght, MinimumLength = TitleMinLenght)]
        public string Title { get; set; } = null!;
        [Required]
        [StringLength(AddressMaxLenght,MinimumLength = AddressMinLenght)]
        public string Address { get; set; } = null!;
        [Required]
        [StringLength(DescriptionMaxLenght, MinimumLength = DescriptionMinLenght)]
        public string Description { get; set; }=null!;
        [Required]
        [StringLength(ImageUrlMaxLenght)]
        [Display(Name = "Image link")]
        public string ImageUrl { get; set; }=null!;
        [Required]
        [Range(typeof (decimal),PriceForMonthMinValue,PriceForMonthMaxValue)]
        [Display(Name = "MonthlyPrice")]
        public decimal PricePerMonth { get; set; }
        [Required]  
        public int CategoryId { get; set; }
        public IEnumerable<HouseSelectCategoryFormModel> Categories { get; set; }




    }
}

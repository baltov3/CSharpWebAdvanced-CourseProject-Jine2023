using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HouseRentingSystem.Coomon.EntityValidationConstants.PhoneNumber;

namespace HouseRentingSystem.Web.ViewModels.Agent
{
    public class BecomeAgentFormModel
    {
        [Required]
        [StringLength(PhoneNumberMaxLenght,MinimumLength = PhoneNumberMinLenght)]
        [Phone]
        [Display(Name ="Phone")]
        public string PhoneNumber { get; set; } = null!;
    }
}

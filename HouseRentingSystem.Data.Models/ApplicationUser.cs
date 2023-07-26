using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HouseRentingSystem.Coomon.EntityValidationConstants.User;

namespace HouseRentingSystem.Data.Models
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public ApplicationUser()
        {
            this.Id = Guid.NewGuid();
            this.RentedHouses= new HashSet<House>();
        }
        [Required]
        [MaxLength(FirstNameMaxLenght)]
        public string FirstName { get; set; } = null!;
        [Required]
        [MaxLength(LastNameMaxLenght)]
        public string LastName { get; set; } =null!;
        public virtual ICollection<House> RentedHouses { get; set; }
    }
}

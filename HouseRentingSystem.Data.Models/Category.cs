using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HouseRentingSystem.Coomon.EntityValidationConstants.Category;

namespace HouseRentingSystem.Data.Models
{
    public class Category
    {
        public Category()
        {
            this.Houses = new HashSet<House>();
        }

        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(CategoryNameMaxLenght)]
        public string Name { get; set; } = null!;
        public virtual ICollection<House> Houses { get; set; }

    }
}

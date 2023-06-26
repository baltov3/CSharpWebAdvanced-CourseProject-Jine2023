using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HouseRentingSystem.Coomon.EntityValidationConstants.House;

namespace HouseRentingSystem.Data.Models
{
    public class House
    {
        public House()
        {
            Id = Guid.NewGuid();
        }
        [Key]
        public Guid Id { get; set; }
        [Required]
        [MaxLength(TitleMaxLenght)]
        public string Title { get; set; } = null!;
        [Required]
        [MaxLength(AddressMaxLenght)]
        public string Address { get; set; } = null!;
        [Required]
        [MaxLength(DescriptionMaxLenght)]
        public string Description { get; set; } = null!;
        [Required]
        [MaxLength(ImageUrlMaxLenght)]
        public string ImageUrl { get; set; } = null!;
        public decimal PricePerMonth { get; set; }
        public DateTime CreatedOn { get; set; }

        public int CategoryId { get; set; }
        public virtual Category Category { get; set; } = null!;

        public Guid AgentId { get; set; }
        public virtual Agent Agent { get; set; } = null!;
        public Guid? RenterId { get; set; }
        public ApplicationUser? Renter { get; set; }
    }
}
